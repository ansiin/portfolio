import type { CreateTaskInput, UpdateTaskInput } from "../../types/dto";
import type { Category, Task } from "../../types/entities";
import type { CategoryRepository } from "../../dal/categoryRepository";
import type { TaskService } from "../../bll/taskService";
import type { AppDom } from "../domBindings";
import { createFilterController } from "./filterController";
import { renderTaskList } from "../render/taskListView";
import {
  closeTaskModal,
  fillCategorySelect,
  fillDependencySelect,
  fillTaskForm,
  openTaskModal,
} from "../render/taskFormView";
import { renderStats } from "../render/statsView";

export type TaskController = {
  init(): Promise<void>;
};

export function createTaskController(
  dom: AppDom,
  taskService: TaskService,
  categoryRepository: CategoryRepository,
): TaskController {
  let categories: Category[] = [];
  let allTasks: Task[] = [];
  let currentTasks: Task[] = [];

  const formInput = (name: string): HTMLInputElement => dom.taskForm.elements.namedItem(name) as HTMLInputElement;
  const formTextarea = (name: string): HTMLTextAreaElement =>
    dom.taskForm.elements.namedItem(name) as HTMLTextAreaElement;
  const formSelect = (name: string): HTMLSelectElement =>
    dom.taskForm.elements.namedItem(name) as HTMLSelectElement;

  const showError = (error: unknown): void => {
    const message = error instanceof Error ? error.message : "Unknown error";
    window.alert(message);
  };

  const fillFilterCategories = (): void => {
    dom.categoryFilter.innerHTML =
      `<option value="">All</option>` +
      categories.map((c) => `<option value="${c.id}">${c.name}</option>`).join("");
  };

  const refreshDependencyOptions = (selectedIds: string[] = [], excludeTaskId?: string): void => {
    const categoryId = formSelect("taskCategory").value || undefined;
    const options: { excludeTaskId?: string; categoryId?: string } = {};
    if (excludeTaskId) {
      options.excludeTaskId = excludeTaskId;
    }
    if (categoryId) {
      options.categoryId = categoryId;
    }
    fillDependencySelect(dom.taskDependencies, allTasks, selectedIds, options);
  };

  const drawTaskList = (): void => {
    renderTaskList(dom.taskList, currentTasks, categories, {
      onEdit: async (id) => {
        try {
          const task = currentTasks.find((item) => item.id === id);
          if (!task) {
            throw new Error("Task not found");
          }
          fillTaskForm(dom.taskForm, task);
          refreshDependencyOptions(task.dependencyIds, task.id);
          openTaskModal(dom.taskModal, dom.modalTitle, "Edit Task");
        } catch (error) {
          showError(error);
        }
      },
      onDelete: async (id) => {
        try {
          await taskService.delete(id);
          await refreshAll();
        } catch (error) {
          showError(error);
        }
      },
      onComplete: async (id) => {
        try {
          await taskService.complete(id);
          await refreshAll();
        } catch (error) {
          showError(error);
        }
      },
    });
  };

  const refreshAll = async (): Promise<void> => {
    allTasks = await taskService.list();
    currentTasks = allTasks;
    drawTaskList();
    refreshDependencyOptions();
    renderStats(dom.statsBox, await taskService.getStats());
  };

  const readTaskInput = (): CreateTaskInput => {
    const title = formInput("taskTitle").value;
    const description = formTextarea("taskDescription").value;
    const status = formSelect("taskStatus").value;
    const priority = formSelect("taskPriority").value;
    const dueDate = formInput("taskDueDate").value;
    const tagsRaw = formInput("taskTags").value;
    const categoryId = formSelect("taskCategory").value;
    const dependencyIds = Array.from(dom.taskDependencies.selectedOptions).map((option) => option.value);
    const recurrenceType = formSelect("taskRecurrenceType").value;
    const recurrenceInterval = formInput("taskRecurrenceInterval").value;
    const recurrenceEndDate = formInput("taskRecurrenceEndDate").value;

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
  };

  const toUpdateInput = (input: CreateTaskInput): UpdateTaskInput => {
    const { title, description, status, priority, dueDate, tags, categoryId, dependencyIds } = input;
    return {
      title,
      ...(description !== undefined ? { description } : {}),
      ...(status !== undefined ? { status } : {}),
      ...(priority !== undefined ? { priority } : {}),
      ...(dueDate !== undefined ? { dueDate } : {}),
      ...(tags !== undefined ? { tags } : {}),
      ...(categoryId !== undefined ? { categoryId } : {}),
      ...(dependencyIds !== undefined ? { dependencyIds } : {}),
    };
  };

  const submitTaskForm = async (): Promise<void> => {
    const taskId = formInput("taskId").value;
    const input = readTaskInput();
    if (taskId) {
      await taskService.update(taskId, toUpdateInput(input));
      return;
    }
    await taskService.add(input);
  };

  const bindEvents = (): void => {
    const closeModal = () => {
      closeTaskModal(dom.taskModal, dom.taskForm);
    };

    dom.addTaskBtn.addEventListener("click", () => {
      dom.taskForm.reset();
      refreshDependencyOptions([]);
      openTaskModal(dom.taskModal, dom.modalTitle, "Add New Task");
    });

    dom.cancelBtn.addEventListener("click", closeModal);
    dom.closeBtn.addEventListener("click", closeModal);

    window.addEventListener("click", (event) => {
      if (event.target === dom.taskModal) {
        closeModal();
      }
    });

    dom.clearTasksBtn.addEventListener("click", async () => {
      if (!window.confirm("Clear all tasks?")) {
        return;
      }
      try {
        await taskService.clear();
        await refreshAll();
      } catch (error) {
        showError(error);
      }
    });

    dom.taskForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      try {
        await submitTaskForm();
        closeModal();
        await refreshAll();
      } catch (error) {
        showError(error);
      }
    });

    formSelect("taskCategory").addEventListener("change", () => {
      const taskId = formInput("taskId").value;
      const selectedDependencies = Array.from(dom.taskDependencies.selectedOptions).map(
        (option) => option.value,
      );
      refreshDependencyOptions(selectedDependencies, taskId || undefined);
    });
  };

  const filterController = createFilterController(dom, async (query) => {
    currentTasks = await taskService.query(query);
    drawTaskList();
  });

  const init = async (): Promise<void> => {
    categories = await categoryRepository.getAll();
    fillCategorySelect(formSelect("taskCategory"), categories);
    fillFilterCategories();
    bindEvents();
    filterController.bind();
    await refreshAll();
  };

  return { init };
}
