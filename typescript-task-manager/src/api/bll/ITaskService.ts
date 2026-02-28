import type { CreateTaskInput, QueryInput, UpdateTaskInput } from "../../types/dto";
import type { Task, TaskStatistics } from "../../types/entities";

export interface ITaskService {
  add(input: CreateTaskInput): Promise<Task>;
  update(id: string, input: UpdateTaskInput): Promise<Task>;
  delete(id: string): Promise<void>;
  list(): Promise<Task[]>;
  clear(): Promise<void>;
  query(query: QueryInput): Promise<Task[]>;
  complete(id: string): Promise<Task>;
  getStats(): Promise<TaskStatistics>;
}
