export type TaskStatus = "todo" | "in-progress" | "completed" | "cancelled";
export type TaskPriority = "low" | "medium" | "high" | "urgent";
export type RecurrenceType = "none" | "daily" | "weekly" | "monthly";

export type Category = {
  id: string;
  name: string;
  createdAt: string;
};

export type RecurrenceRule = {
  type: RecurrenceType;
  interval: number;
  endDate?: string;
};

export type Task = {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate?: string;
  tags: string[];
  categoryId?: string;
  dependencyIds: string[];
  recurrence: RecurrenceRule;
  createdAt: string;
  updatedAt: string;
};

export type TaskStatistics = {
  total: number;
  completed: number;
  overdue: number;
  completionRate: number;
  byStatus: Record<TaskStatus, number>;
  byPriority: Record<TaskPriority, number>;
};

export type SortOption = {
  field: "title" | "createdAt" | "dueDate" | "priority" | "status";
  direction: "asc" | "desc";
};
