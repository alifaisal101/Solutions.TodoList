import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import AuthCard from './AuthCard';

export default function LoginForm() {
    const { login, error, loading } = useAuth();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [formError, setFormError] = useState('');
    const navigate = useNavigate();

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setFormError('');
        if (!username.trim() || !password) {
            setFormError('Both fields are required.');
            return;
        }
        try {
            await login(username.trim(), password);
            navigate('/', { replace: true });
        } catch (_) {
            // context sets error already; keep silent here
        }
    }

    return (
        <AuthCard
            title="Sign in"
            footer={
                <div className="auth-switch">
                    <span>New here?</span>
                    <Link to="/register" className="link">Create an account</Link>
                </div>
            }
        >
            <form onSubmit={handleSubmit} className="auth-form">
                {(formError || error) && (
                    <div className="form-error">{formError || error}</div>
                )}

                <label className="form-field">
                    <span className="label">Username</span>
                    <input
                        className="input"
                        type="text"
                        placeholder="username"
                        value={username}
                        onChange={e => setUsername(e.target.value)}
                        autoComplete="username"
                    />
                </label>

                <label className="form-field">
                    <span className="label">Password</span>
                    <input
                        className="input"
                        type="password"
                        placeholder="••••••••"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        autoComplete="current-password"
                    />
                </label>

                <button className="btn" type="submit" disabled={loading}>
                    {loading ? 'Signing in…' : 'Sign in'}
                </button>
            </form>
        </AuthCard>
    );
}
