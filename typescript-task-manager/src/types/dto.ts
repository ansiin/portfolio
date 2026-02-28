import type { SortOption, TaskPriority, TaskStatus } from "./entities";

export interface CreateTaskInput {
  title: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDate?: string;
  tags?: string[];
  categoryId?: string;
  dependencyIds?: string[];
  recurrenceType?: "none" | "daily" | "weekly" | "monthly";
  recurrenceInterval?: number;
  recurrenceEndDate?: string;
}

export interface UpdateTaskInput {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDate?: string;
  tags?: string[];
  categoryId?: string;
  dependencyIds?: string[];
}

export interface TaskFilterInput {
  status?: TaskStatus;
  priority?: TaskPriority;
  categoryId?: string;
  dueDate?: string;
  tag?: string;
}

export interface QueryInput {
  search?: string;
  filter?: TaskFilterInput;
  sort?: SortOption;
}
