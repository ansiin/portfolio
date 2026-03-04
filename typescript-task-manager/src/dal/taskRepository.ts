import type { Task } from "../types/entities";
import type { StorageClient } from "./storageClient";

const TASKS_KEY = "tasks";

export type TaskRepository = {
  getAll(): Promise<Task[]>;
  getById(id: string): Promise<Task | null>;
  save(task: Task): Promise<void>;
  remove(id: string): Promise<void>;
  clear(): Promise<void>;
};

export function createTaskRepository(storage: StorageClient): TaskRepository {
  const getAll = async (): Promise<Task[]> => {
    return storage.read<Task[]>(TASKS_KEY, []);
  };

  const getById = async (id: string): Promise<Task | null> => {
    const tasks = await getAll();
    return tasks.find((task) => task.id === id) ?? null;
  };

  const save = async (task: Task): Promise<void> => {
    const tasks = await getAll();
    const index = tasks.findIndex((item) => item.id === task.id);
    if (index >= 0) {
      tasks[index] = task;
    } else {
      tasks.push(task);
    }
    await storage.write(TASKS_KEY, tasks);
  };

  const remove = async (id: string): Promise<void> => {
    const tasks = await getAll();
    await storage.write(
      TASKS_KEY,
      tasks.filter((task) => task.id !== id),
    );
  };

  const clear = async (): Promise<void> => {
    await storage.write(TASKS_KEY, []);
  };

  return { getAll, getById, save, remove, clear };
}
