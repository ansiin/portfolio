import type { Task } from "../../types/entities";

export interface IDependencyService {
  hasCycle(tasks: Task[], taskId: string, dependencyIds: string[]): boolean;
  canMarkCompleted(tasks: Task[], task: Task): boolean;
}
