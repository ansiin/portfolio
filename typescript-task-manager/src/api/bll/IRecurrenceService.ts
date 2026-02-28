import type { Task } from "../../types/entities";

export interface IRecurrenceService {
  createNext(task: Task): Task | null;
}
