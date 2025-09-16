import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function TopNav() {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate('/login', { replace: true });
    }

    return (
        <header className="topnav">
            <div className="container" style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <div style={{ display: 'flex', gap: 16, alignItems: 'center' }}>
                    <Link to="/" className="brand">MyApp</Link>
                </div>

                <nav>
                    {user ? (
                        <>
                            <Link to="/">Home</Link>
                            <button onClick={handleLogout} className="link" style={{ marginLeft: 12 }}>
                                Logout ({user.username})
                            </button>
                        </>
                    ) : (
                        <>
                            <Link to="/login">Sign in</Link>
                            <Link to="/register" style={{ marginLeft: 12 }}>Register</Link>
                        </>
                    )}
                </nav>
            </div>
        </header>
    );
}