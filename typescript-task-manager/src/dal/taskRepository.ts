import type { ITaskRepository } from "../api/dal/ITaskRepository";
import type { IStorageClient } from "../api/dal/IStorageClient";
import type { Task } from "../types/entities";

const TASKS_KEY = "tasks";

export class TaskRepository implements ITaskRepository {
  constructor(private readonly storage: IStorageClient) {}

  async getAll(): Promise<Task[]> {
    return this.storage.read<Task[]>(TASKS_KEY, []);
  }

  async getById(id: string): Promise<Task | null> {
    const tasks = await this.getAll();
    return tasks.find((task) => task.id === id) ?? null;
  }

  async save(task: Task): Promise<void> {
    const tasks = await this.getAll();
    const index = tasks.findIndex((item) => item.id === task.id);
    if (index >= 0) {
      tasks[index] = task;
    } else {
      tasks.push(task);
    }
    await this.storage.write(TASKS_KEY, tasks);
  }

  async remove(id: string): Promise<void> {
    const tasks = await this.getAll();
    await this.storage.write(
      TASKS_KEY,
      tasks.filter((task) => task.id !== id),
    );
  }

  async clear(): Promise<void> {
    await this.storage.write(TASKS_KEY, []);
  }
}
