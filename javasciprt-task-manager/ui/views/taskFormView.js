import { normalizeTags } from "../../shared/utils.js";

export function openTaskModal(dom, title, task = null) {
  dom.modalTitle.textContent = title;
  if (task) {
    dom.taskForm.querySelector("#taskId").value = task.id;
    dom.taskForm.querySelector("#taskTitle").value = task.title;
    dom.taskForm.querySelector("#taskDescription").value = task.description || "";
    dom.taskForm.querySelector("#taskStatus").value = task.status;
    dom.taskForm.querySelector("#taskPriority").value = task.priority;
    dom.taskForm.querySelector("#taskDueDate").value = task.dueDate ? task.dueDate.split("T")[0] : "";
    dom.taskForm.querySelector("#taskTags").value = (task.tags || []).join(", ");
  } else {
    dom.taskForm.reset();
  }
  dom.taskModal.style.display = "block";
}

export function closeTaskModal(dom) {
  dom.taskModal.style.display = "none";
  dom.taskForm.reset();
}

export function readTaskForm(dom) {
  return {
    title: dom.taskForm.querySelector("#taskTitle").value,
    description: dom.taskForm.querySelector("#taskDescription").value,
    status: dom.taskForm.querySelector("#taskStatus").value,
    priority: dom.taskForm.querySelector("#taskPriority").value,
    dueDate: dom.taskForm.querySelector("#taskDueDate").value || null,
    tags: normalizeTags(dom.taskForm.querySelector("#taskTags").value),
  };
}
