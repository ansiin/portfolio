import type { QueryInput } from "../../types/dto";
import type { Task } from "../../types/entities";

export interface IQueryService {
  run(tasks: Task[], query: QueryInput): Task[];
}
