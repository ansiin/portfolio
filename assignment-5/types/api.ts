export type JwtResponse = {
  token: string | null;
  refreshToken: string | null;
  firstName: string | null;
  lastName: string | null;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = LoginRequest & {
  firstName: string;
  lastName: string;
};

export type RefreshTokenRequest = {
  jwt: string;
  refreshToken: string;
};

export type ApiMessage = {
  messages?: string[] | null;
  title?: string | null;
  detail?: string | null;
};

export type TodoCategory = {
  id: string;
  categoryName: string | null;
  categorySort: number;
  syncDt: string;
  tag: string | null;
};

export type TodoCategoryCreate = {
  categoryName: string | null;
  categorySort: number;
  tag: string | null;
};

export type TodoPriority = {
  id: string;
  priorityName: string | null;
  prioritySort: number;
  syncDt: string;
};

export type TodoPriorityCreate = {
  priorityName: string | null;
  prioritySort: number;
  syncDt: string;
};

export type TodoTask = {
  id: string;
  taskName: string | null;
  taskSort: number;
  createdDt: string;
  dueDt: string | null;
  isCompleted: boolean;
  isArchived: boolean;
  todoCategoryId: string;
  todoPriorityId: string;
  syncDt: string;
};

export type TodoTaskCreate = {
  taskName: string | null;
  taskSort: number;
  createdDt: string;
  dueDt: string | null;
  isCompleted: boolean;
  isArchived: boolean;
  todoCategoryId: string;
  todoPriorityId: string;
};
