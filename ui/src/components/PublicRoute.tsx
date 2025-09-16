import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function PublicRoute({ children }: { children: ReactNode }) {
    const { user, loading } = useAuth();
    if (loading) return <div className="center">Loading…</div>;
    if (user) return <Navigate to="/" replace />;
    return <>{children}</>;
}
