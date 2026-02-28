import { ValidationError } from "./errors.js";

export const VALID_STATUSES = ["todo", "in-progress", "completed", "cancelled"];
export const VALID_PRIORITIES = ["low", "medium", "high", "urgent"];

function validateTitle(title) {
  if (!title || typeof title !== "string" || title.trim().length === 0) {
    throw new ValidationError("Title is required");
  }
  if (title.length > 200) {
    throw new ValidationError("Title cannot exceed 200 characters");
  }
}

function validateDescription(description) {
  if (description && typeof description !== "string") {
    throw new ValidationError("Description must be a string");
  }
  if (description && description.length > 1000) {
    throw new ValidationError("Description cannot exceed 1000 characters");
  }
}

function validateStatus(status) {
  if (!status || !VALID_STATUSES.includes(String(status).toLowerCase())) {
    throw new ValidationError(`Status must be one of: ${VALID_STATUSES.join(", ")}`);
  }
}

function validatePriority(priority) {
  if (!priority || !VALID_PRIORITIES.includes(String(priority).toLowerCase())) {
    throw new ValidationError(`Priority must be one of: ${VALID_PRIORITIES.join(", ")}`);
  }
}

function validateDueDate(dueDate) {
  if (dueDate && Number.isNaN(new Date(dueDate).getTime())) {
    throw new ValidationError("Due date must be a valid date");
  }
}

function validateTags(tags) {
  if (tags && !Array.isArray(tags)) {
    throw new ValidationError("Tags must be an array");
  }
  (tags || []).forEach((tag, index) => {
    if (typeof tag !== "string") {
      throw new ValidationError(`Tag at index ${index} must be a string`);
    }
    if (!tag.trim()) {
      throw new ValidationError(`Tag at index ${index} cannot be empty`);
    }
    if (tag.length > 50) {
      throw new ValidationError(`Tag at index ${index} cannot exceed 50 characters`);
    }
  });
}

export function validateTaskData(taskData) {
  validateTitle(taskData.title);
  validateDescription(taskData.description);
  validateStatus(taskData.status || "todo");
  validatePriority(taskData.priority || "medium");
  validateDueDate(taskData.dueDate);
  validateTags(taskData.tags);
}
