import { NotFoundError } from "../shared/errors.js";
import { generateId, normalizeTags } from "../shared/utils.js";
import { validateTaskData } from "../shared/validators.js";

export function createTaskService({
  taskRepository,
  queryService,
  statsService,
  dependencyService,
  recurrenceService,
}) {
  function createTask(taskData) {
    const now = new Date().toISOString();
    return {
      id: taskData.id || generateId(),
      title: taskData.title,
      description: taskData.description || "",
      status: taskData.status || "todo",
      priority: taskData.priority || "medium",
      dueDate: taskData.dueDate || null,
      tags: normalizeTags(taskData.tags),
      createdAt: now,
      updatedAt: now,
    };
  }

  function add(taskData) {
    const normalized = { ...taskData, tags: normalizeTags(taskData.tags) };
    validateTaskData(normalized);
    return taskRepository.save(createTask(normalized));
  }

  function update(taskId, updates) {
    const existing = taskRepository.getById(taskId);
    if (!existing) {
      throw new NotFoundError(`Task with ID ${taskId} not found`);
    }
    const updated = {
      ...existing,
      ...updates,
      tags: updates.tags ? normalizeTags(updates.tags) : existing.tags,
      updatedAt: new Date().toISOString(),
    };
    validateTaskData(updated);
    return taskRepository.save(updated);
  }

  function remove(taskId) {
    const existing = taskRepository.getById(taskId);
    if (!existing) {
      throw new NotFoundError(`Task with ID ${taskId} not found`);
    }
    taskRepository.remove(taskId);
  }

  function list() {
    return taskRepository.getAll();
  }

  function getById(taskId) {
    const task = taskRepository.getById(taskId);
    if (!task) {
      throw new NotFoundError(`Task with ID ${taskId} not found`);
    }
    return task;
  }

  function clearAll() {
    taskRepository.clear();
  }

  function filter(filterCriteria) {
    return queryService.filter(taskRepository.getAll(), filterCriteria);
  }

  function search(query) {
    return queryService.search(taskRepository.getAll(), query);
  }

  function complete(taskId) {
    const existing = getById(taskId);
    if (!dependencyService.canComplete(existing)) {
      return existing;
    }
    const updated = update(taskId, { status: "completed" });
    const next = recurrenceService.maybeCreateNextTask(updated);
    if (next) {
      taskRepository.save(next);
    }
    return updated;
  }

  function stats() {
    return statsService.calculate(taskRepository.getAll());
  }

  return {
    add,
    update,
    remove,
    list,
    getById,
    clearAll,
    filter,
    search,
    complete,
    stats,
  };
}
