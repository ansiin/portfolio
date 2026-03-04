import type { SortOption, TaskPriority, TaskStatus } from "./entities";

export type CreateTaskInput = {
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
};

export type UpdateTaskInput = {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDate?: string;
  tags?: string[];
  categoryId?: string;
  dependencyIds?: string[];
};

export type TaskFilterInput = {
  status?: TaskStatus;
  priority?: TaskPriority;
  categoryId?: string;
  dueDate?: string;
  tag?: string;
};

export type QueryInput = {
  search?: string;
  filter?: TaskFilterInput;
  sort?: SortOption;
};
