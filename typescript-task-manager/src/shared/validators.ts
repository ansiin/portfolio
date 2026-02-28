import type { CreateTaskInput, UpdateTaskInput } from "../types/dto";
import type { TaskPriority, TaskStatus } from "../types/entities";
import { ValidationError } from "./errors";

const VALID_STATUSES: TaskStatus[] = [
  "todo",
  "in-progress",
  "completed",
  "cancelled",
];
const VALID_PRIORITIES: TaskPriority[] = ["low", "medium", "high", "urgent"];
const VALID_RECURRENCES = ["none", "daily", "weekly", "monthly"] as const;

function isDateValue(date: string): boolean {
  return !Number.isNaN(new Date(date).getTime());
}

export function validateTaskCreate(input: CreateTaskInput): void {
  if (!input.title || input.title.trim().length === 0) {
    throw new ValidationError("Title is required");
  }
  if (input.title.length > 200) {
    throw new ValidationError("Title max length is 200");
  }
  if (input.description && input.description.length > 1000) {
    throw new ValidationError("Description max length is 1000");
  }
  if (input.status && !VALID_STATUSES.includes(input.status)) {
    throw new ValidationError(`Invalid status: ${input.status}`);
  }
  if (input.priority && !VALID_PRIORITIES.includes(input.priority)) {
    throw new ValidationError(`Invalid priority: ${input.priority}`);
  }
  if (input.dueDate && !isDateValue(input.dueDate)) {
    throw new ValidationError("Due date is invalid");
  }
  if (input.recurrenceType && !VALID_RECURRENCES.includes(input.recurrenceType)) {
    throw new ValidationError("Recurrence type is invalid");
  }
  if (input.recurrenceInterval !== undefined && input.recurrenceInterval < 1) {
    throw new ValidationError("Recurrence interval must be >= 1");
  }
  if (input.recurrenceEndDate && !isDateValue(input.recurrenceEndDate)) {
    throw new ValidationError("Recurrence end date is invalid");
  }
}

export function validateTaskUpdate(input: UpdateTaskInput): void {
  if (input.title !== undefined && input.title.trim().length === 0) {
    throw new ValidationError("Title cannot be empty");
  }
  if (input.title && input.title.length > 200) {
    throw new ValidationError("Title max length is 200");
  }
  if (input.description && input.description.length > 1000) {
    throw new ValidationError("Description max length is 1000");
  }
  if (input.status && !VALID_STATUSES.includes(input.status)) {
    throw new ValidationError(`Invalid status: ${input.status}`);
  }
  if (input.priority && !VALID_PRIORITIES.includes(input.priority)) {
    throw new ValidationError(`Invalid priority: ${input.priority}`);
  }
  if (input.dueDate && !isDateValue(input.dueDate)) {
    throw new ValidationError("Due date is invalid");
  }
}
