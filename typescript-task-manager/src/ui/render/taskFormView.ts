import type { Category, Task } from "../../types/entities";

export function fillCategorySelect(select: HTMLSelectElement, categories: Category[]): void {
  const current = select.value;
  select.innerHTML =
    `<option value="">None</option>` +
    categories.map((category) => `<option value="${category.id}">${category.name}</option>`).join("");
  select.value = current;
}

export function openTaskModal(modal: HTMLElement, modalTitle: HTMLElement, title: string): void {
  modalTitle.textContent = title;
  modal.style.display = "block";
}

export function closeTaskModal(modal: HTMLElement, form: HTMLFormElement): void {
  modal.style.display = "none";
  form.reset();
}

export function fillTaskForm(form: HTMLFormElement, task: Task): void {
  (form.elements.namedItem("taskId") as HTMLInputElement).value = task.id;
  (form.elements.namedItem("taskTitle") as HTMLInputElement).value = task.title;
  (form.elements.namedItem("taskDescription") as HTMLTextAreaElement).value = task.description;
  (form.elements.namedItem("taskStatus") as HTMLSelectElement).value = task.status;
  (form.elements.namedItem("taskPriority") as HTMLSelectElement).value = task.priority;
  (form.elements.namedItem("taskDueDate") as HTMLInputElement).value = task.dueDate
    ? task.dueDate.slice(0, 10)
    : "";
  (form.elements.namedItem("taskTags") as HTMLInputElement).value = task.tags.join(", ");
  (form.elements.namedItem("taskCategory") as HTMLSelectElement).value = task.categoryId ?? "";
  (form.elements.namedItem("taskRecurrenceType") as HTMLSelectElement).value = task.recurrence.type;
  (form.elements.namedItem("taskRecurrenceInterval") as HTMLInputElement).value = String(task.recurrence.interval);
  (form.elements.namedItem("taskRecurrenceEndDate") as HTMLInputElement).value = task.recurrence.endDate
    ? task.recurrence.endDate.slice(0, 10)
    : "";
}

export function fillDependencySelect(
  select: HTMLSelectElement,
  tasks: Task[],
  selectedIds: string[],
  options?: { excludeTaskId?: string; categoryId?: string },
): void {
  const list = tasks.filter((task) => {
    if (options?.excludeTaskId && task.id === options.excludeTaskId) {
      return false;
    }
    if (options?.categoryId) {
      return task.categoryId === options.categoryId;
    }
    return true;
  });

  select.innerHTML = list
    .map((task) => {
      const selected = selectedIds.includes(task.id) ? " selected" : "";
      return `<option value="${task.id}"${selected}>${task.title} (${task.id}) [${task.status}]</option>`;
    })
    .join("");
}
