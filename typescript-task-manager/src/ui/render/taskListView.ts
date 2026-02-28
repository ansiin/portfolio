import type { Category, Task } from "../../types/entities";

interface TaskListHandlers {
  onEdit: (id: string) => void;
  onDelete: (id: string) => void;
  onComplete: (id: string) => void;
}

export function renderTaskList(
  root: HTMLElement,
  tasks: Task[],
  categories: Category[],
  handlers: TaskListHandlers,
): void {
  if (tasks.length === 0) {
    root.innerHTML = `
      <div class="empty-state">
        <h3>No tasks found</h3>
        <p>Add a task to get started.</p>
      </div>
    `;
    return;
  }

  const categoryMap = new Map(categories.map((c) => [c.id, c.name]));
  root.innerHTML = tasks
    .map((task) => {
      const categoryName = task.categoryId ? (categoryMap.get(task.categoryId) ?? "Unknown") : "None";
      return `
        <div class="task-card">
          <div class="task-header">
            <h3 class="task-title">${task.title}</h3>
            <div class="task-id">${task.id}</div>
          </div>
          <div class="task-details">
            <span class="status-${task.status}">${task.status}</span>
            <span class="priority-${task.priority}">${task.priority}</span>
            <span>Category: ${categoryName}</span>
            ${task.dueDate ? `<span>Due: ${new Date(task.dueDate).toLocaleDateString()}</span>` : "<span>Due: none</span>"}
          </div>
          ${task.description ? `<div class="task-description">${task.description}</div>` : ""}
          ${task.tags.length > 0 ? `<div class="task-tags">${task.tags.map((tag) => `<span class="tag">${tag}</span>`).join("")}</div>` : ""}
          ${
            task.dependencyIds.length > 0
              ? `<div class="task-description">Depends on: ${task.dependencyIds.join(", ")}</div>`
              : ""
          }
          ${
            task.recurrence.type !== "none"
              ? `<div class="task-description">Recurring: ${task.recurrence.type} (${task.recurrence.interval})</div>`
              : ""
          }
          <div class="task-actions">
            <button data-action="complete" data-id="${task.id}">Complete</button>
            <button data-action="edit" data-id="${task.id}">Edit</button>
            <button data-action="delete" data-id="${task.id}">Delete</button>
          </div>
        </div>
      `;
    })
    .join("");

  root.querySelectorAll<HTMLButtonElement>("[data-action]").forEach((button) => {
    const action = button.dataset.action;
    const id = button.dataset.id;
    if (!action || !id) {
      return;
    }
    button.addEventListener("click", () => {
      if (action === "edit") {
        handlers.onEdit(id);
      } else if (action === "delete") {
        handlers.onDelete(id);
      } else if (action === "complete") {
        handlers.onComplete(id);
      }
    });
  });
}
