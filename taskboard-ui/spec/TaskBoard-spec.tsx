/// <reference types="vitest/globals" />
/// <reference types="@testing-library/jest-dom" />
import '@testing-library/jest-dom/vitest';

import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import TaskBoard from '../src/TaskBoard';
import { createTask, deleteTask, getTasks, updateTask } from '../src/api.ts';

vi.mock('../src/api.ts', () => ({
	getTasks: vi.fn(),
	createTask: vi.fn(),
	updateTask: vi.fn(),
	deleteTask: vi.fn(),
}));

const mockedGetTasks = vi.mocked(getTasks);
const mockedCreateTask = vi.mocked(createTask);
const mockedUpdateTask = vi.mocked(updateTask);
const mockedDeleteTask = vi.mocked(deleteTask);

describe('TaskBoard', () => {
	beforeEach(() => {
		mockedGetTasks.mockReset();
		mockedCreateTask.mockReset();
		mockedUpdateTask.mockReset();
		mockedDeleteTask.mockReset();
	});

	it('loads and displays tasks', async () => {
		mockedGetTasks.mockResolvedValue([
			{ id: 1, title: 'Draft kickoff brief', isComplete: false },
			{ id: 2, title: 'Review storyboard', isComplete: true },
		]);

		render(<TaskBoard />);

		expect(screen.getByText('Loading tasks...')).toBeInTheDocument();

		expect(
			await screen.findByText('Draft kickoff brief')
		).toBeInTheDocument();
		expect(screen.getByText('Review storyboard')).toBeInTheDocument();
		expect(screen.getAllByText(/open|complete/i).length).toBeGreaterThan(0);
	});

	it('creates a task from the form', async () => {
		mockedGetTasks.mockResolvedValue([]);
		mockedCreateTask.mockResolvedValue({
			id: 7,
			title: 'Prep sprint demo',
			isComplete: false,
		});

		render(<TaskBoard />);

		await screen.findByText('No tasks yet. Add the first one.');

		const input = screen.getByPlaceholderText('Design onboarding flow');
		await userEvent.type(input, 'Prep sprint demo');
		await userEvent.click(screen.getByRole('button', { name: 'Add task' }));

		await waitFor(() => {
			expect(mockedCreateTask).toHaveBeenCalledWith('Prep sprint demo');
		});
		expect(await screen.findByText('Prep sprint demo')).toBeInTheDocument();
	});

	it('renames an existing task', async () => {
		mockedGetTasks.mockResolvedValue([
			{ id: 4, title: 'Write release notes', isComplete: false },
		]);
		mockedUpdateTask.mockResolvedValue({
			id: 4,
			title: 'Publish release notes',
			isComplete: false,
		});

		render(<TaskBoard />);

		await screen.findByText('Write release notes');
		await userEvent.click(screen.getByRole('button', { name: 'Rename' }));

		const editInput = screen.getByDisplayValue('Write release notes');
		await userEvent.clear(editInput);
		await userEvent.type(editInput, 'Publish release notes');
		await userEvent.click(screen.getByRole('button', { name: 'Save' }));

		await waitFor(() => {
			expect(mockedUpdateTask).toHaveBeenCalledWith(
				4,
				'Publish release notes'
			);
		});
		expect(
			await screen.findByText('Publish release notes')
		).toBeInTheDocument();
	});

	it('deletes a task', async () => {
		mockedGetTasks.mockResolvedValue([
			{ id: 10, title: 'Archive backlog', isComplete: false },
		]);
		mockedDeleteTask.mockResolvedValue();

		render(<TaskBoard />);

		await screen.findByText('Archive backlog');
		await userEvent.click(screen.getByRole('button', { name: 'Delete' }));

		await waitFor(() => {
			expect(mockedDeleteTask).toHaveBeenCalledWith(10);
		});
		await waitFor(() => {
			expect(screen.queryByText('Archive backlog')).toBeNull();
		});
	});

	it('shows an error when loading fails', async () => {
		mockedGetTasks.mockRejectedValue(new Error('Network down'));

		render(<TaskBoard />);

		expect(await screen.findByText('Network down')).toBeInTheDocument();
	});
});
