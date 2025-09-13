export interface AuthResponse {
    id: string;
    username: string;
    role: string;
    accessToken: string;
}

export interface Todo {
    id: string;
    title: string;
    description: string;
    done: boolean;
    createdAtUtc: string;
    updatedAtUtc: string | null;
    completedAtUtc: string | null;
  }
  
export interface TodoListResponse {
    data: {
      items: Todo[];
      totalCount: number;
    };
    meta: any;
    success: boolean;
    message: string | null;
}
  
  export interface TodoResponse {
    data: Todo;
    meta: any;
    success: boolean;
    message: string | null;
  }