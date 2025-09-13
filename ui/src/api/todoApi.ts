import type { TodoListResponse, TodoResponse } from "./../types";

const API_URL = import.meta.env.VITE_API_URL ?? '/api/v1';

export async function getTodos(token: string, skip = 0, take = 20) {
  const res = await fetch(`${API_URL}/todos?sort=createdAt_desc&skip=${skip}&take=${take}`, {
    credentials: 'include',
    headers: { 'Authorization': `Bearer ${token}` }
  });
  if (!res.ok) throw new Error('Failed to fetch todos');
  return res.json() as Promise<TodoListResponse>;
}

export async function createTodo(token: string, title: string, description: string) {
  const res = await fetch(`${API_URL}/todos`, {
    method: 'POST',
    credentials: 'include',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ title, description })
  });
  if (!res.ok) throw new Error('Failed to create todo');
  return res.json() as Promise<TodoResponse>;
}
