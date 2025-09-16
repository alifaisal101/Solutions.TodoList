import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import * as todoApi from "../../api/todoApi";
import type { Todo } from "../../types";

export default function TodoList() {
    const { accessToken } = useAuth();
    const [todos, setTodos] = useState<Todo[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");

    async function fetchTodos() {
        if (!accessToken) return;
        setLoading(true);
        setError(null);
        try {
            const res = await todoApi.getTodos(accessToken);
            setTodos(res.data.items);
        } catch (err: any) {
            setError(err.message || "Failed to load todos");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        fetchTodos();
        // eslint-disable-next-line
    }, [accessToken]);

    async function handleAddTodo(e: React.FormEvent) {
        e.preventDefault();
        if (!title.trim()) {
            setError("Title required");
            return;
        }
        setLoading(true);
        setError(null);
        try {
            const res = await todoApi.createTodo(accessToken!, title.trim(), description.trim());
            setTodos((prev) => [res.data, ...prev]);
            setTitle("");
            setDescription("");
        } catch (err: any) {
            setError(err.message || "Failed to add todo");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="todo-root">
            <form onSubmit={handleAddTodo} className="todo-form" aria-label="Add todo">
                <div className="todo-form-row">
                    <input
                        className="input"
                        type="text"
                        placeholder="Title"
                        value={title}
                        onChange={e => setTitle(e.target.value)}
                        aria-label="Todo title"
                    />
                    <input
                        className="input"
                        type="text"
                        placeholder="Description (optional)"
                        value={description}
                        onChange={e => setDescription(e.target.value)}
                        aria-label="Todo description"
                    />
                    <button className="btn" type="submit" disabled={loading}>Add</button>
                </div>
                {error && <div className="form-error" role="status" style={{ marginTop: 10 }}>{error}</div>}
            </form>

            <div className="todo-list">
                {loading ? (
                    <div className="center">Loading…</div>
                ) : todos.length === 0 ? (
                    <div className="placeholder">No todos yet — add one above.</div>
                ) : (
                    <ul className="todo-items" aria-live="polite">
                        {todos.map(todo =>
                            <li key={todo.id} className="todo-item">
                                <div className="todo-item__left">
                                    <div className="todo-item__title">{todo.title}</div>
                                    {todo.description && <div className="todo-item__desc">{todo.description}</div>}
                                </div>
                                <div className="todo-item__right">
                                    {todo.done ? <span className="todo-badge">Done</span> : null}
                                </div>
                            </li>
                        )}
                    </ul>
                )}
            </div>
        </div>
    );
}
