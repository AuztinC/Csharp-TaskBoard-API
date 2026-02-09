const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

export type Task = {
    id: number;
    title: string;
    isComplete: boolean;
};

type ApiError = {
    error?: string;
};

async function readJson<T>(response: Response): Promise<T> {
    const text = await response.text();
    if (!text) {
        return {} as T;
    }
    return JSON.parse(text) as T;
}

function toErrorMessage(status: number, payload?: ApiError): string {
    if (payload?.error) {
        return payload.error;
    }
    return `Request failed (${status})`;
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: {
            'Content-Type': 'application/json',
            ...(options?.headers ?? {}),
        },
        ...options,
    });

    if (!response.ok) {
        const payload = await readJson<ApiError>(response);
        throw new Error(toErrorMessage(response.status, payload));
    }

    if (response.status === 204) {
        return {} as T;
    }

    return readJson<T>(response);
}

export function getTasks(): Promise<Task[]> {
    return request<Task[]>('/api/tasks');
}

export function getTask(id: number): Promise<Task> {
    return request<Task>(`/api/tasks/${id}`);
}

export function createTask(title: string): Promise<Task> {
    return request<Task>('/api/tasks', {
        method: 'POST',
        body: JSON.stringify({ title }),
    });
}

export function updateTask(id: number, title: string): Promise<Task> {
    return request<Task>(`/api/tasks/${id}`, {
        method: 'PUT',
        body: JSON.stringify({ title }),
    });
}

export function deleteTask(id: number): Promise<void> {
    return request<void>(`/api/tasks/${id}`, {
        method: 'DELETE',
    });
}