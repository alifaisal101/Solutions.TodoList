import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import AuthCard from './AuthCard';

export default function RegisterForm() {
    const { register, error, loading } = useAuth();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirm, setConfirm] = useState('');
    const [formError, setFormError] = useState('');
    const navigate = useNavigate();

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setFormError('');
        if (!username.trim() || !password) {
            setFormError('Both fields are required.');
            return;
        }
        if (password !== confirm) {
            setFormError('Passwords do not match.');
            return;
        }
        try {
            await register(username.trim(), password);
            navigate('/', { replace: true });
        } catch (_) {
            // error handled by context
        }
    }

    return (
        <AuthCard
            title="Create account"
            footer={
                <div className="auth-switch">
                    <span>Already have an account?</span>
                    <Link to="/login" className="link">Sign in</Link>
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
                        autoComplete="new-password"
                    />
                </label>

                <label className="form-field">
                    <span className="label">Confirm password</span>
                    <input
                        className="input"
                        type="password"
                        placeholder="confirm password"
                        value={confirm}
                        onChange={e => setConfirm(e.target.value)}
                        autoComplete="new-password"
                    />
                </label>

                <button className="btn" type="submit" disabled={loading}>
                    {loading ? 'Creating…' : 'Create account'}
                </button>
            </form>
        </AuthCard>
    );
}
