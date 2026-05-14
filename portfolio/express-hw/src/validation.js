export function isNonEmptyString(value, maxLength = 128) {
  return typeof value === "string" && value.trim().length > 0 && value.length <= maxLength;
}

export function isInt32(value) {
  return Number.isInteger(value) && value >= -2147483648 && value <= 2147483647;
}

export function asDateString(value, fallback = new Date().toISOString()) {
  if (value === null || value === undefined || value === "") return fallback;
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? fallback : date.toISOString();
}

export function validateCategoryCreate(body) {
  const errors = {};
  if (!isNonEmptyString(body.categoryName)) errors.categoryName = ["categoryName is required and must be at most 128 characters."];
  if (!isInt32(body.categorySort)) errors.categorySort = ["categorySort must be a 32-bit integer."];
  return errors;
}

export function validatePriorityCreate(body) {
  const errors = {};
  if (!isNonEmptyString(body.priorityName)) errors.priorityName = ["priorityName is required and must be at most 128 characters."];
  if (!isInt32(body.prioritySort)) errors.prioritySort = ["prioritySort must be a 32-bit integer."];
  return errors;
}

export function validateTaskCreate(body) {
  const errors = {};
  if (!isNonEmptyString(body.taskName)) errors.taskName = ["taskName is required and must be at most 128 characters."];
  if (!isInt32(body.taskSort)) errors.taskSort = ["taskSort must be a 32-bit integer."];
  if (!isNonEmptyString(body.todoCategoryId, 64)) errors.todoCategoryId = ["todoCategoryId is required."];
  if (!isNonEmptyString(body.todoPriorityId, 64)) errors.todoPriorityId = ["todoPriorityId is required."];
  return errors;
}

export function hasErrors(errors) {
  return Object.keys(errors).length > 0;
}
