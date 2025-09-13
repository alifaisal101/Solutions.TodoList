import type { AuthResponse } from '../types';

const API_URL = import.meta.env.VITE_API_URL ?? '/api/v1';

async function post(path: string, body?: unknown): Promise<Response> {
  return fetch(`${API_URL}${path}`, {
    method: 'POST',
    credentials: 'include',
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
}

async function readError(res: Response, fallback: string): Promise<Error> {
  try {
    const problem = await res.json();
    return new Error(problem.detail ?? problem.title ?? fallback);
  } catch {
    return new Error(fallback);
  }
}

export async function register(username: string, password: string): Promise<AuthResponse> {
  const res = await post('/auth/register', { username, password });
  if (!res.ok) throw await readError(res, 'Registration failed');
  return res.json();
}

export async function login(username: string, password: string): Promise<AuthResponse> {
  const res = await post('/auth/login', { username, password });
  if (!res.ok) throw await readError(res, 'Login failed');
  return res.json();
}

export async function refresh(): Promise<AuthResponse> {
  const res = await post('/auth/refresh');
  if (!res.ok) throw new Error('Refresh failed');
  return res.json();
}

export async function logout(): Promise<void> {
  await post('/auth/logout');
}

export async function getMe(accessToken: string) {
  const res = await fetch(`${API_URL}/auth/me`, {
    credentials: 'include',
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  if (!res.ok) throw new Error('GetMe failed');
  return res.json();
}
