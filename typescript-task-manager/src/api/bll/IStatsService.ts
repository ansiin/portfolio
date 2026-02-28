import type { Task, TaskStatistics } from "../../types/entities";

export interface IStatsService {
  calculate(tasks: Task[]): TaskStatistics;
}
