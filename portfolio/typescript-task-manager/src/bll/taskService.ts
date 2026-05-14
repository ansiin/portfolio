import type { CreateTaskInput, QueryInput, UpdateTaskInput } from "../types/dto";
import type { Task, TaskStatistics } from "../types/entities";
import type { CategoryRepository } from "../dal/categoryRepository";
import type { TaskRepository } from "../dal/taskRepository";
import { dependencyError, notFoundError, validationError } from "../shared/errors";
import { generateId, uniqueBy } from "../shared/utils";
import { validateTaskCreate, validateTaskUpdate } from "../shared/validators";
import { canMarkTaskCompleted, hasDependencyCycle } from "./dependencyService";
import { runTaskQuery } from "./queryService";
import { createNextRecurringTask } from "./recurrenceService";
import { calculateTaskStats } from "./statsService";

export type TaskService = {
  add(input: CreateTaskInput): Promise<Task>;
  update(id: string, input: UpdateTaskInput): Promise<Task>;
  delete(id: string): Promise<void>;
  list(): Promise<Task[]>;
  clear(): Promise<void>;
  query(query: QueryInput): Promise<Task[]>;
  complete(id: string): Promise<Task>;
  getStats(): Promise<TaskStatistics>;
};

export function createTaskService(tasks: TaskRepository, categories: CategoryRepository): TaskService {
  const ensureCategoryExists = async (categoryId?: string): Promise<void> => {
    if (!categoryId) {
      return;
    }
    const category = await categories.getById(categoryId);
    if (!category) {
      throw validationError(`Category not found: ${categoryId}`);
    }
  };

  const ensureDependenciesExist = async (dependencyIds: string[]): Promise<void> => {
    const all = await tasks.getAll();
    for (const depId of dependencyIds) {
      if (!all.some((task) => task.id === depId)) {
        throw validationError(`Dependency not found: ${depId}`);
      }
    }
  };

  const add = async (input: CreateTaskInput): Promise<Task> => {
    validateTaskCreate(input);
    await ensureCategoryExists(input.categoryId);
    await ensureDependenciesExist(input.dependencyIds ?? []);

    const now = new Date().toISOString();
    const recurrence = {
      type: input.recurrenceType ?? "none",
      interval: input.recurrenceInterval ?? 1,
      ...(input.recurrenceEndDate ? { endDate: input.recurrenceEndDate } : {}),
    };
    const task: Task = {
      id: generateId(),
      title: input.title.trim(),
      description: input.description?.trim() ?? "",
      status: input.status ?? "todo",
      priority: input.priority ?? "medium",
      tags: uniqueBy((input.tags ?? []).map((tag) => tag.trim()).filter(Boolean), (tag) => tag.toLowerCase()),
      dependencyIds: input.dependencyIds ?? [],
      recurrence,
      ...(input.dueDate ? { dueDate: input.dueDate } : {}),
      ...(input.categoryId ? { categoryId: input.categoryId } : {}),
      createdAt: now,
      updatedAt: now,
    };

    const all = await tasks.getAll();
    if (hasDependencyCycle(all, task.id, task.dependencyIds)) {
      throw dependencyError("Dependency cycle detected");
    }

    await tasks.save(task);
    return task;
  };

  const update = async (id: string, input: UpdateTaskInput): Promise<Task> => {
    validateTaskUpdate(input);
    const existing = await tasks.getById(id);
    if (!existing) {
      throw notFoundError(`Task not found: ${id}`);
    }

    await ensureCategoryExists(input.categoryId);
    await ensureDependenciesExist(input.dependencyIds ?? existing.dependencyIds);

    const updated: Task = {
      ...existing,
      ...input,
      title: input.title?.trim() ?? existing.title,
      description: input.description?.trim() ?? existing.description,
      tags: input.tags
        ? uniqueBy(input.tags.map((tag) => tag.trim()).filter(Boolean), (tag) => tag.toLowerCase())
        : existing.tags,
      updatedAt: new Date().toISOString(),
    };

    const all = await tasks.getAll();
    if (hasDependencyCycle(all, id, updated.dependencyIds)) {
      throw dependencyError("Dependency cycle detected");
    }

    await tasks.save(updated);
    return updated;
  };

  const remove = async (id: string): Promise<void> => {
    const existing = await tasks.getById(id);
    if (!existing) {
      throw notFoundError(`Task not found: ${id}`);
    }

    const all = await tasks.getAll();
    const dependent = all.some((task) => task.dependencyIds.includes(id));
    if (dependent) {
      throw dependencyError("Cannot delete task that other tasks depend on");
    }

    await tasks.remove(id);
  };

  const list = async (): Promise<Task[]> => tasks.getAll();

  const clear = async (): Promise<void> => {
    await tasks.clear();
  };

  const query = async (input: QueryInput): Promise<Task[]> => {
    const all = await tasks.getAll();
    return runTaskQuery(all, input);
  };

  const complete = async (id: string): Promise<Task> => {
    const task = await tasks.getById(id);
    if (!task) {
      throw notFoundError(`Task not found: ${id}`);
    }

    const all = await tasks.getAll();
    if (!canMarkTaskCompleted(all, task)) {
      throw dependencyError("Task dependencies are not completed");
    }

    const updated: Task = {
      ...task,
      status: "completed",
      updatedAt: new Date().toISOString(),
    };
    await tasks.save(updated);

    const next = createNextRecurringTask(updated);
    if (next) {
      await tasks.save(next);
    }

    return updated;
  };

  const getStats = async (): Promise<TaskStatistics> => {
    const all = await tasks.getAll();
    return calculateTaskStats(all);
  };

  return {
    add,
    update,
    delete: remove,
    list,
    clear,
    query,
    complete,
    getStats,
  };
}
