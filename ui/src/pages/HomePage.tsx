import TodoList from '../components/Todos/TodoList';
import { useAuth } from '../context/AuthContext';

export default function HomePage() {
    const { user } = useAuth();

    return (
        <main className="home">
            <div className="home__card">
                <h2 className="home__title">Welcome, {user?.username}</h2>
                <p className="home__subtitle">This is your dashboard. Put your todo list component below.</p>

                <TodoList />
            </div>
        </main>
    );
}
