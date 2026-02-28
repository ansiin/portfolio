import type { ITaskService } from "../api/bll/ITaskService";
import type { IDependencyService } from "../api/bll/IDependencyService";
import type { IQueryService } from "../api/bll/IQueryService";
import type { IRecurrenceService } from "../api/bll/IRecurrenceService";
import type { IStatsService } from "../api/bll/IStatsService";
import type { ICategoryRepository } from "../api/dal/ICategoryRepository";
import type { ITaskRepository } from "../api/dal/ITaskRepository";
import type { CreateTaskInput, QueryInput, UpdateTaskInput } from "../types/dto";
import type { Task, TaskStatistics } from "../types/entities";
import { DependencyError, NotFoundError, ValidationError } from "../shared/errors";
import { generateId, uniqueBy } from "../shared/utils";
import { validateTaskCreate, validateTaskUpdate } from "../shared/validators";

export class TaskService implements ITaskService {
  constructor(
    private readonly tasks: ITaskRepository,
    private readonly categories: ICategoryRepository,
    private readonly dependencies: IDependencyService,
    private readonly recurrence: IRecurrenceService,
    private readonly stats: IStatsService,
    private readonly queryService: IQueryService,
  ) {}

  async add(input: CreateTaskInput): Promise<Task> {
    validateTaskCreate(input);
    await this.ensureCategoryExists(input.categoryId);
    await this.ensureDependenciesExist(input.dependencyIds ?? []);

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

    const all = await this.tasks.getAll();
    if (this.dependencies.hasCycle(all, task.id, task.dependencyIds)) {
      throw new DependencyError("Dependency cycle detected");
    }

    await this.tasks.save(task);
    return task;
  }

  async update(id: string, input: UpdateTaskInput): Promise<Task> {
    validateTaskUpdate(input);
    const existing = await this.tasks.getById(id);
    if (!existing) {
      throw new NotFoundError(`Task not found: ${id}`);
    }

    await this.ensureCategoryExists(input.categoryId);
    await this.ensureDependenciesExist(input.dependencyIds ?? existing.dependencyIds);

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

    const all = await this.tasks.getAll();
    if (this.dependencies.hasCycle(all, id, updated.dependencyIds)) {
      throw new DependencyError("Dependency cycle detected");
    }

    await this.tasks.save(updated);
    return updated;
  }

  async delete(id: string): Promise<void> {
    const existing = await this.tasks.getById(id);
    if (!existing) {
      throw new NotFoundError(`Task not found: ${id}`);
    }

    const all = await this.tasks.getAll();
    const dependent = all.some((task) => task.dependencyIds.includes(id));
    if (dependent) {
      throw new DependencyError("Cannot delete task that other tasks depend on");
    }
    await this.tasks.remove(id);
  }

  async list(): Promise<Task[]> {
    return this.tasks.getAll();
  }

  async clear(): Promise<void> {
    await this.tasks.clear();
  }

  async query(query: QueryInput): Promise<Task[]> {
    const all = await this.tasks.getAll();
    return this.queryService.run(all, query);
  }

  async complete(id: string): Promise<Task> {
    const task = await this.tasks.getById(id);
    if (!task) {
      throw new NotFoundError(`Task not found: ${id}`);
    }

    const all = await this.tasks.getAll();
    if (!this.dependencies.canMarkCompleted(all, task)) {
      throw new DependencyError("Task dependencies are not completed");
    }

    const updated: Task = {
      ...task,
      status: "completed",
      updatedAt: new Date().toISOString(),
    };
    await this.tasks.save(updated);

    const next = this.recurrence.createNext(updated);
    if (next) {
      await this.tasks.save(next);
    }

    return updated;
  }

  async getStats(): Promise<TaskStatistics> {
    const all = await this.tasks.getAll();
    return this.stats.calculate(all);
  }

  private async ensureCategoryExists(categoryId?: string): Promise<void> {
    if (!categoryId) {
      return;
    }
    const category = await this.categories.getById(categoryId);
    if (!category) {
      throw new ValidationError(`Category not found: ${categoryId}`);
    }
  }

  private async ensureDependenciesExist(dependencyIds: string[]): Promise<void> {
    const all = await this.tasks.getAll();
    for (const depId of dependencyIds) {
      if (!all.some((task) => task.id === depId)) {
        throw new ValidationError(`Dependency not found: ${depId}`);
      }
    }
  }
}
