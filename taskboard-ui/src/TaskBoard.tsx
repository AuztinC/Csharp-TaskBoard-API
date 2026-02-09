import { type SubmitEvent, useEffect, useMemo, useState } from 'react';
import { type Task, createTask, deleteTask, getTasks, updateTask } from './api.ts';

export default function TaskBoard() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [newTitle, setNewTitle] = useState('');
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editingTitle, setEditingTitle] = useState('');
  const sortedTasks = useMemo(
    () => [...tasks].sort((a, b) => a.id - b.id),
    [tasks]
  );

  useEffect(() => {
    void fetchTasks();
  }, []);

  async function fetchTasks() {
    setIsLoading(true);
    setError(null);
    try {
      const data = await getTasks();
      setTasks(data);
    } catch (fetchError) {
      const message =
        fetchError instanceof Error
          ? fetchError.message
          : 'Unable to load tasks.';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }

  async function handleCreate(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = newTitle.trim();
    if (!trimmed) {
      setError('Title cannot be empty.');
      return;
    }

    setError(null);
    try {
      const created = await createTask(trimmed);
      setTasks((prev) => [...prev, created]);
      setNewTitle('');
    } catch (createError) {
      const message =
        createError instanceof Error
          ? createError.message
          : 'Unable to create task.';
      setError(message);
    }
  }

  function startEditing(task: Task) {
    setEditingId(task.id);
    setEditingTitle(task.title);
  }

  function cancelEditing() {
    setEditingId(null);
    setEditingTitle('');
  }

  async function handleUpdate(task: Task) {
    const trimmed = editingTitle.trim();
    if (!trimmed) {
      setError('Title cannot be empty.');
      return;
    }

    setError(null);
    try {
      const updated = await updateTask(task.id, trimmed);
      setTasks((prev) =>
        prev.map((item) => (item.id === task.id ? updated : item))
      );
      cancelEditing();
    } catch (updateError) {
      const message =
        updateError instanceof Error
          ? updateError.message
          : 'Unable to update task.';
      setError(message);
    }
  }

  async function handleDelete(task: Task) {
    setError(null);
    try {
      await deleteTask(task.id);
      setTasks((prev) => prev.filter((item) => item.id !== task.id));
    } catch (deleteError) {
      const message =
        deleteError instanceof Error
          ? deleteError.message
          : 'Unable to delete task.';
      setError(message);
    }
  }

  return (
    <div className="task-board">
      <header className="task-board__header">
        <div>
          <p className="task-board__eyebrow">TaskBoard</p>
          <h1>Keep work moving.</h1>
          <p className="task-board__subhead">
            Add, rename, and review what is ready next.
          </p>
        </div>
        <button className="ghost-button" type="button" onClick={fetchTasks}>
          Refresh
        </button>
      </header>

      <section className="task-board__panel">
        <form className="task-board__form" onSubmit={handleCreate}>
          <label className="field">
            <span>New task</span>
            <input
              value={newTitle}
              onChange={(event) => setNewTitle(event.target.value)}
              placeholder="Design onboarding flow"
              maxLength={120}
            />
          </label>
          <button type="submit" className="primary-button">
            Add task
          </button>
        </form>

        {error ? <p className="task-board__error">{error}</p> : null}

        {isLoading ? (
          <p className="task-board__status">Loading tasks...</p>
        ) : sortedTasks.length === 0 ? (
          <p className="task-board__status">No tasks yet. Add the first one.</p>
        ) : (
          <ul className="task-board__list">
            {sortedTasks.map((task) => (
              <li key={task.id} className="task-card">
                <div className="task-card__title">
                  <span className={task.isComplete ? 'done' : undefined}>
                    {task.title}
                  </span>
                  {task.isComplete ? (
                    <span className="task-card__badge">Complete</span>
                  ) : (
                    <span className="task-card__badge pending">Open</span>
                  )}
                </div>

                {editingId === task.id ? (
                  <div className="task-card__edit">
                    <input
                      value={editingTitle}
                      onChange={(event) =>
                        setEditingTitle(event.target.value)
                      }
                      maxLength={120}
                    />
                    <div className="task-card__actions">
                      <button
                        type="button"
                        className="primary-button"
                        onClick={() => handleUpdate(task)}
                      >
                        Save
                      </button>
                      <button
                        type="button"
                        className="ghost-button"
                        onClick={cancelEditing}
                      >
                        Cancel
                      </button>
                    </div>
                  </div>
                ) : (
                  <div className="task-card__actions">
                    <button
                      type="button"
                      className="ghost-button"
                      onClick={() => startEditing(task)}
                    >
                      Rename
                    </button>
                    <button
                      type="button"
                      className="danger-button"
                      onClick={() => handleDelete(task)}
                    >
                      Delete
                    </button>
                  </div>
                )}
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}