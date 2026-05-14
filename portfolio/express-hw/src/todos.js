import { v4 as uuid } from "uuid";
import { getStore, saveStore } from "./store.js";
import { asDateString, hasErrors, validateCategoryCreate, validatePriorityCreate, validateTaskCreate } from "./validation.js";
import { problem } from "./problem.js";

function owned(items, userId) {
  return items.filter((item) => item.userId === userId);
}

function withoutOwner(item) {
  const { userId, ...dto } = item;
  return dto;
}

function findOwned(items, userId, id) {
  return items.find((item) => item.userId === userId && item.id === id);
}

function notFound(res) {
  return problem(res, 404, "Not Found", "Resource was not found.");
}

export async function listCategories(req, res) {
  const store = await getStore();
  return res.json(owned(store.todoCategories, req.user.id).map(withoutOwner));
}

export async function createCategory(req, res) {
  const errors = validateCategoryCreate(req.body || {});
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  const store = await getStore();
  const category = {
    id: uuid(),
    userId: req.user.id,
    categoryName: req.body.categoryName,
    categorySort: req.body.categorySort,
    syncDt: new Date().toISOString(),
    tag: req.body.tag ?? null
  };

  store.todoCategories.push(category);
  await saveStore();
  return res.status(201).json(withoutOwner(category));
}

export async function getCategory(req, res) {
  const store = await getStore();
  const category = findOwned(store.todoCategories, req.user.id, req.params.id);
  return category ? res.json(withoutOwner(category)) : notFound(res);
}

export async function updateCategory(req, res) {
  const errors = validateCategoryCreate(req.body || {});
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  const store = await getStore();
  const category = findOwned(store.todoCategories, req.user.id, req.params.id);
  if (!category) return notFound(res);

  category.categoryName = req.body.categoryName;
  category.categorySort = req.body.categorySort;
  category.syncDt = new Date().toISOString();
  category.tag = req.body.tag ?? null;
  await saveStore();
  return res.json(withoutOwner(category));
}

export async function deleteCategory(req, res) {
  const store = await getStore();
  const before = store.todoCategories.length;
  store.todoCategories = store.todoCategories.filter((item) => !(item.userId === req.user.id && item.id === req.params.id));
  store.todoTasks = store.todoTasks.filter((item) => !(item.userId === req.user.id && item.todoCategoryId === req.params.id));
  if (store.todoCategories.length === before) return notFound(res);
  await saveStore();
  return res.status(204).send();
}

export async function listPriorities(req, res) {
  const store = await getStore();
  return res.json(owned(store.todoPriorities, req.user.id).map(withoutOwner));
}

export async function createPriority(req, res) {
  const errors = validatePriorityCreate(req.body || {});
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  const store = await getStore();
  const priority = {
    id: uuid(),
    userId: req.user.id,
    priorityName: req.body.priorityName,
    prioritySort: req.body.prioritySort,
    syncDt: asDateString(req.body.syncDt)
  };

  store.todoPriorities.push(priority);
  await saveStore();
  return res.json(withoutOwner(priority));
}

export async function getPriority(req, res) {
  const store = await getStore();
  const priority = findOwned(store.todoPriorities, req.user.id, req.params.id);
  return priority ? res.json(withoutOwner(priority)) : notFound(res);
}

export async function updatePriority(req, res) {
  const errors = validatePriorityCreate(req.body || {});
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  const store = await getStore();
  const priority = findOwned(store.todoPriorities, req.user.id, req.params.id);
  if (!priority) return notFound(res);

  priority.priorityName = req.body.priorityName;
  priority.prioritySort = req.body.prioritySort;
  priority.syncDt = asDateString(req.body.syncDt);
  await saveStore();
  return res.json(withoutOwner(priority));
}

export async function deletePriority(req, res) {
  const store = await getStore();
  const before = store.todoPriorities.length;
  store.todoPriorities = store.todoPriorities.filter((item) => !(item.userId === req.user.id && item.id === req.params.id));
  store.todoTasks = store.todoTasks.filter((item) => !(item.userId === req.user.id && item.todoPriorityId === req.params.id));
  if (store.todoPriorities.length === before) return notFound(res);
  await saveStore();
  return res.status(200).send();
}

export async function listTasks(req, res) {
  const store = await getStore();
  return res.json(owned(store.todoTasks, req.user.id).map(withoutOwner));
}

export async function createTask(req, res) {
  const errors = validateTaskCreate(req.body || {});
  const store = await getStore();
  const category = findOwned(store.todoCategories, req.user.id, req.body?.todoCategoryId);
  const priority = findOwned(store.todoPriorities, req.user.id, req.body?.todoPriorityId);

  if (!category) errors.todoCategoryId = ["todoCategoryId must reference an existing category."];
  if (!priority) errors.todoPriorityId = ["todoPriorityId must reference an existing priority."];
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  const task = {
    id: uuid(),
    userId: req.user.id,
    taskName: req.body.taskName,
    taskSort: req.body.taskSort,
    createdDt: asDateString(req.body.createdDt),
    dueDt: req.body.dueDt ? asDateString(req.body.dueDt, null) : null,
    isCompleted: Boolean(req.body.isCompleted),
    isArchived: Boolean(req.body.isArchived),
    todoCategoryId: req.body.todoCategoryId,
    todoPriorityId: req.body.todoPriorityId,
    syncDt: new Date().toISOString()
  };

  store.todoTasks.push(task);
  await saveStore();
  return res.json(withoutOwner(task));
}

export async function getTask(req, res) {
  const store = await getStore();
  const task = findOwned(store.todoTasks, req.user.id, req.params.id);
  return task ? res.json(withoutOwner(task)) : notFound(res);
}

export async function updateTask(req, res) {
  const errors = validateTaskCreate(req.body || {});
  const store = await getStore();
  const task = findOwned(store.todoTasks, req.user.id, req.params.id);
  const category = findOwned(store.todoCategories, req.user.id, req.body?.todoCategoryId);
  const priority = findOwned(store.todoPriorities, req.user.id, req.body?.todoPriorityId);

  if (!task) return notFound(res);
  if (!category) errors.todoCategoryId = ["todoCategoryId must reference an existing category."];
  if (!priority) errors.todoPriorityId = ["todoPriorityId must reference an existing priority."];
  if (hasErrors(errors)) return problem(res, 400, "One or more validation errors occurred.", null, errors);

  task.taskName = req.body.taskName;
  task.taskSort = req.body.taskSort;
  task.createdDt = asDateString(req.body.createdDt);
  task.dueDt = req.body.dueDt ? asDateString(req.body.dueDt, null) : null;
  task.isCompleted = Boolean(req.body.isCompleted);
  task.isArchived = Boolean(req.body.isArchived);
  task.todoCategoryId = req.body.todoCategoryId;
  task.todoPriorityId = req.body.todoPriorityId;
  task.syncDt = asDateString(req.body.syncDt);
  await saveStore();
  return res.json(withoutOwner(task));
}

export async function deleteTask(req, res) {
  const store = await getStore();
  const before = store.todoTasks.length;
  store.todoTasks = store.todoTasks.filter((item) => !(item.userId === req.user.id && item.id === req.params.id));
  if (store.todoTasks.length === before) return notFound(res);
  await saveStore();
  return res.status(200).send();
}
