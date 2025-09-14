import { createContext, useContext, useState, useEffect } from 'react';
import * as authApi from '../api/authApi';
import type { ReactNode } from 'react';
import type { AuthResponse } from '../types';

interface User {
    id: string;
    username: string;
    role: string;
}

interface AuthContextType {
    user: User | null;
    accessToken: string | null;
    login: (username: string, password: string) => Promise<void>;
    register: (username: string, password: string) => Promise<void>;
    logout: () => void;
    loading: boolean;
    error: string | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [accessToken, setAccessToken] = useState<string | null>(localStorage.getItem('accessToken'));
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    function applySession(res: AuthResponse) {
        setAccessToken(res.accessToken);
        localStorage.setItem('accessToken', res.accessToken);
        setUser({ id: res.id, username: res.username, role: res.role });
    }

    function clearSession() {
        setUser(null);
        setAccessToken(null);
        localStorage.removeItem('accessToken');
    }

    async function handleLoginOrRegister(
        fn: typeof authApi.login | typeof authApi.register,
        username: string,
        password: string
    ) {
        setError(null);
        setLoading(true);
        try {
            applySession(await fn(username, password));
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Authentication failed');
            throw err;
        } finally {
            setLoading(false);
        }
    }

    const login = (username: string, password: string) =>
        handleLoginOrRegister(authApi.login, username, password);

    const register = (username: string, password: string) =>
        handleLoginOrRegister(authApi.register, username, password);

    function logout() {
        authApi.logout().catch(() => { /* best-effort server revoke */ });
        clearSession();
    }

    // On mount: restore the session from the access token, falling back to the
    // HttpOnly refresh cookie (set by the API) when it is missing or expired.
    useEffect(() => {
        let active = true;

        (async () => {
            try {
                let token = accessToken ?? (await authApi.refresh()).accessToken;
                let me;
                try {
                    me = await authApi.getMe(token);
                } catch {
                    token = (await authApi.refresh()).accessToken;
                    me = await authApi.getMe(token);
                }
                if (!active) return;
                setAccessToken(token);
                localStorage.setItem('accessToken', token);
                setUser({ id: me.id, username: me.username, role: me.role });
            } catch {
                if (active) clearSession();
            } finally {
                if (active) setLoading(false);
            }
        })();

        return () => { active = false; };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <AuthContext.Provider value={{ user, accessToken, login, register, logout, loading, error }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
}
