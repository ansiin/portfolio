import type { CreateTaskInput, UpdateTaskInput } from "../types/dto";
import type { TaskPriority, TaskStatus } from "../types/entities";
import { validationError } from "./errors";

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
    throw validationError("Title is required");
  }
  if (input.title.length > 200) {
    throw validationError("Title max length is 200");
  }
  if (input.description && input.description.length > 1000) {
    throw validationError("Description max length is 1000");
  }
  if (input.status && !VALID_STATUSES.includes(input.status)) {
    throw validationError(`Invalid status: ${input.status}`);
  }
  if (input.priority && !VALID_PRIORITIES.includes(input.priority)) {
    throw validationError(`Invalid priority: ${input.priority}`);
  }
  if (input.dueDate && !isDateValue(input.dueDate)) {
    throw validationError("Due date is invalid");
  }
  if (input.recurrenceType && !VALID_RECURRENCES.includes(input.recurrenceType)) {
    throw validationError("Recurrence type is invalid");
  }
  if (input.recurrenceInterval !== undefined && input.recurrenceInterval < 1) {
    throw validationError("Recurrence interval must be >= 1");
  }
  if (input.recurrenceEndDate && !isDateValue(input.recurrenceEndDate)) {
    throw validationError("Recurrence end date is invalid");
  }
}

export function validateTaskUpdate(input: UpdateTaskInput): void {
  if (input.title !== undefined && input.title.trim().length === 0) {
    throw validationError("Title cannot be empty");
  }
  if (input.title && input.title.length > 200) {
    throw validationError("Title max length is 200");
  }
  if (input.description && input.description.length > 1000) {
    throw validationError("Description max length is 1000");
  }
  if (input.status && !VALID_STATUSES.includes(input.status)) {
    throw validationError(`Invalid status: ${input.status}`);
  }
  if (input.priority && !VALID_PRIORITIES.includes(input.priority)) {
    throw validationError(`Invalid priority: ${input.priority}`);
  }
  if (input.dueDate && !isDateValue(input.dueDate)) {
    throw validationError("Due date is invalid");
  }
}
