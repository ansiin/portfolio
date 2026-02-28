import type { ITaskController } from "../../api/ui/ITaskController";
import type { ICategoryRepository } from "../../api/dal/ICategoryRepository";
import type { ITaskService } from "../../api/bll/ITaskService";
import type { CreateTaskInput, UpdateTaskInput } from "../../types/dto";
import type { Category, Task } from "../../types/entities";
import { FilterController } from "./filterController";
import type { AppDom } from "../domBindings";
import { renderTaskList } from "../render/taskListView";
import {
  closeTaskModal,
  fillCategorySelect,
  fillDependencySelect,
  fillTaskForm,
  openTaskModal,
} from "../render/taskFormView";
import { renderStats } from "../render/statsView";

export class TaskController implements ITaskController {
  private categories: Category[] = [];
  private allTasks: Task[] = [];
  private currentTasks: Task[] = [];
  private readonly filterController: FilterController;

  constructor(
    private readonly dom: AppDom,
    private readonly taskService: ITaskService,
    private readonly categoryRepository: ICategoryRepository,
  ) {
    this.filterController = new FilterController(dom, async (query) => {
      this.currentTasks = await this.taskService.query(query);
      this.drawTaskList();
    });
  }

  async init(): Promise<void> {
    this.categories = await this.categoryRepository.getAll();
    fillCategorySelect(
      this.dom.taskForm.elements.namedItem("taskCategory") as HTMLSelectElement,
      this.categories,
    );
    this.fillFilterCategories();
    this.bindEvents();
    this.filterController.bind();
    await this.refreshAll();
  }

  private bindEvents(): void {
    this.dom.addTaskBtn.addEventListener("click", () => {
      this.dom.taskForm.reset();
      this.refreshDependencyOptions([]);
      openTaskModal(this.dom.taskModal, this.dom.modalTitle, "Add New Task");
    });

    this.dom.cancelBtn.addEventListener("click", () => {
      closeTaskModal(this.dom.taskModal, this.dom.taskForm);
    });

    this.dom.closeBtn.addEventListener("click", () => {
      closeTaskModal(this.dom.taskModal, this.dom.taskForm);
    });

    window.addEventListener("click", (event) => {
      if (event.target === this.dom.taskModal) {
        closeTaskModal(this.dom.taskModal, this.dom.taskForm);
      }
    });

    this.dom.clearTasksBtn.addEventListener("click", async () => {
      if (!window.confirm("Clear all tasks?")) {
        return;
      }
      try {
        await this.taskService.clear();
        await this.refreshAll();
      } catch (error) {
        this.showError(error);
      }
    });

    this.dom.taskForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      try {
        await this.submitTaskForm();
        closeTaskModal(this.dom.taskModal, this.dom.taskForm);
        await this.refreshAll();
      } catch (error) {
        this.showError(error);
      }
    });

    const categorySelect = this.dom.taskForm.elements.namedItem("taskCategory") as HTMLSelectElement;
    categorySelect.addEventListener("change", () => {
      const taskId = (this.dom.taskForm.elements.namedItem("taskId") as HTMLInputElement).value;
      const selectedDependencies = Array.from(this.dom.taskDependencies.selectedOptions).map(
        (option) => option.value,
      );
      this.refreshDependencyOptions(selectedDependencies, taskId || undefined);
    });
  }

  private async refreshAll(): Promise<void> {
    this.allTasks = await this.taskService.list();
    this.currentTasks = this.allTasks;
    this.drawTaskList();
    this.refreshDependencyOptions();
    renderStats(this.dom.statsBox, await this.taskService.getStats());
  }

  private drawTaskList(): void {
    renderTaskList(this.dom.taskList, this.currentTasks, this.categories, {
      onEdit: async (id) => {
        try {
          const task = this.currentTasks.find((item) => item.id === id);
          if (!task) {
            throw new Error("Task not found");
          }
          fillTaskForm(this.dom.taskForm, task);
          this.refreshDependencyOptions(task.dependencyIds, task.id);
          openTaskModal(this.dom.taskModal, this.dom.modalTitle, "Edit Task");
        } catch (error) {
          this.showError(error);
        }
      },
      onDelete: async (id) => {
        try {
          await this.taskService.delete(id);
          await this.refreshAll();
        } catch (error) {
          this.showError(error);
        }
      },
      onComplete: async (id) => {
        try {
          await this.taskService.complete(id);
          await this.refreshAll();
        } catch (error) {
          this.showError(error);
        }
      },
    });
  }

  private async submitTaskForm(): Promise<void> {
    const taskId = (this.dom.taskForm.elements.namedItem("taskId") as HTMLInputElement).value;
    const input = this.readTaskInput();
    if (taskId) {
      const updateInput: UpdateTaskInput = {};
      updateInput.title = input.title;
      if (input.description !== undefined) {
        updateInput.description = input.description;
      }
      if (input.status !== undefined) {
        updateInput.status = input.status;
      }
      if (input.priority !== undefined) {
        updateInput.priority = input.priority;
      }
      if (input.dueDate !== undefined) {
        updateInput.dueDate = input.dueDate;
      }
      if (input.tags !== undefined) {
        updateInput.tags = input.tags;
      }
      if (input.categoryId !== undefined) {
        updateInput.categoryId = input.categoryId;
      }
      if (input.dependencyIds !== undefined) {
        updateInput.dependencyIds = input.dependencyIds;
      }
      await this.taskService.update(taskId, updateInput);
      return;
    }
    await this.taskService.add(input);
  }

  private readTaskInput(): CreateTaskInput {
    const title = (this.dom.taskForm.elements.namedItem("taskTitle") as HTMLInputElement).value;
    const description = (this.dom.taskForm.elements.namedItem("taskDescription") as HTMLTextAreaElement).value;
    const status = (this.dom.taskForm.elements.namedItem("taskStatus") as HTMLSelectElement).value;
    const priority = (this.dom.taskForm.elements.namedItem("taskPriority") as HTMLSelectElement).value;
    const dueDate = (this.dom.taskForm.elements.namedItem("taskDueDate") as HTMLInputElement).value;
    const tagsRaw = (this.dom.taskForm.elements.namedItem("taskTags") as HTMLInputElement).value;
    const categoryId = (this.dom.taskForm.elements.namedItem("taskCategory") as HTMLSelectElement).value;
    const dependencyIds = Array.from(this.dom.taskDependencies.selectedOptions).map((option) => option.value);
    const recurrenceType = (this.dom.taskForm.elements.namedItem("taskRecurrenceType") as HTMLSelectElement).value;
    const recurrenceInterval = (this.dom.taskForm.elements.namedItem("taskRecurrenceInterval") as HTMLInputElement).value;
    const recurrenceEndDate = (this.dom.taskForm.elements.namedItem("taskRecurrenceEndDate") as HTMLInputElement).value;

    const createInput: CreateTaskInput = {
      title,
      description,
      status: status as NonNullable<CreateTaskInput["status"]>,
      priority: priority as NonNullable<CreateTaskInput["priority"]>,
      tags: tagsRaw
        .split(",")
        .map((tag) => tag.trim())
        .filter((tag) => tag.length > 0),
      dependencyIds,
      recurrenceType: recurrenceType as NonNullable<CreateTaskInput["recurrenceType"]>,
      recurrenceInterval: Number(recurrenceInterval) || 1,
    };
    if (dueDate) {
      createInput.dueDate = dueDate;
    }
    if (categoryId) {
      createInput.categoryId = categoryId;
    }
    if (recurrenceEndDate) {
      createInput.recurrenceEndDate = recurrenceEndDate;
    }
    return createInput;
  }

  private fillFilterCategories(): void {
    this.dom.categoryFilter.innerHTML =
      `<option value="">All</option>` +
      this.categories.map((c) => `<option value="${c.id}">${c.name}</option>`).join("");
  }

  private refreshDependencyOptions(selectedIds: string[] = [], excludeTaskId?: string): void {
    const categoryId = (this.dom.taskForm.elements.namedItem("taskCategory") as HTMLSelectElement).value || undefined;
    const options: { excludeTaskId?: string; categoryId?: string } = {};
    if (excludeTaskId) {
      options.excludeTaskId = excludeTaskId;
    }
    if (categoryId) {
      options.categoryId = categoryId;
    }
    fillDependencySelect(this.dom.taskDependencies, this.allTasks, selectedIds, options);
  }

  private showError(error: unknown): void {
    const message = error instanceof Error ? error.message : "Unknown error";
    window.alert(message);
  }
}
