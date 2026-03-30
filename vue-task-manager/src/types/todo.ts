export interface ApiMessage {
  message?: string;
  messages?: string[];
}

export interface TodoCategory {
  id: string;
  categoryName: string;
  categorySort: number;
  syncDt: string;
  tag?: string | null;
}

export interface TodoCategoryCreatePayload {
  id: string;
  categoryName: string;
  categorySort: number;
  tag?: string | null;
}

export interface TodoCategoryUpdatePayload extends TodoCategoryCreatePayload {
  syncDt?: string | null;
}

export interface TodoPriority {
  id: string;
  appUserId?: string;
  priorityName: string;
  prioritySort: number;
  syncDt: string;
  tag?: string | null;
}

export interface TodoPriorityPayload {
  id: string;
  priorityName: string;
  prioritySort: number;
  syncDt: string;
  tag?: string | null;
}

export interface TodoTask {
  id: string;
  taskName: string;
  taskSort: number;
  createdDt: string;
  dueDt?: string | null;
  isCompleted: boolean;
  isArchived: boolean;
  todoCategoryId: string;
  todoPriorityId: string;
  syncDt: string;
}

export interface TodoTaskPayload {
  id: string;
  taskName: string;
  taskSort: number;
  createdDt: string;
  dueDt?: string | null;
  isCompleted: boolean;
  isArchived: boolean;
  todoCategoryId: string;
  todoPriorityId: string;
  syncDt: string;
}
