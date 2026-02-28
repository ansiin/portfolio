import type { Task } from "../../types/entities";

export interface ITaskRepository {
  getAll(): Promise<Task[]>;
  getById(id: string): Promise<Task | null>;
  save(task: Task): Promise<void>;
  remove(id: string): Promise<void>;
  clear(): Promise<void>;
}
