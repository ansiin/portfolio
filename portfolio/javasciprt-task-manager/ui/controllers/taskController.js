import { createFilterController } from "./filterController.js";
import { closeTaskModal, openTaskModal, readTaskForm } from "../views/taskFormView.js";
import { renderTaskList } from "../views/taskListView.js";

export function createTaskController(dom, taskService) {
  let visibleTasks = [];

  function refresh(tasks = taskService.list()) {
    visibleTasks = tasks;
    renderTaskList(dom.taskList, visibleTasks);
  }

  function onTaskListClick(event) {
    const button = event.target.closest("button[data-action]");
    if (!button) {
      return;
    }
    const taskId = button.dataset.id;
    if (!taskId) {
      return;
    }

    try {
      if (button.dataset.action === "edit") {
        const task = taskService.getById(taskId);
        openTaskModal(dom, "Edit Task", task);
        return;
      }
      if (button.dataset.action === "delete") {
        taskService.remove(taskId);
        refresh();
      }
    } catch (error) {
      alert(error.message);
    }
  }

  function onSubmit(event) {
    event.preventDefault();
    try {
      const taskId = dom.taskForm.querySelector("#taskId").value;
      const data = readTaskForm(dom);
      if (taskId) {
        taskService.update(taskId, data);
      } else {
        taskService.add(data);
      }
      closeTaskModal(dom);
      refresh();
    } catch (error) {
      alert(error.message);
    }
  }

  function bind() {
    const filterController = createFilterController(dom, taskService, refresh);
    filterController.bind();

    dom.taskForm.addEventListener("submit", onSubmit);
    dom.taskList.addEventListener("click", onTaskListClick);

    dom.addTaskBtn.addEventListener("click", () => openTaskModal(dom, "Add New Task"));
    dom.cancelBtn.addEventListener("click", () => closeTaskModal(dom));
    dom.closeBtn.addEventListener("click", () => closeTaskModal(dom));

    dom.clearTasksBtn.addEventListener("click", () => {
      if (!confirm("Are you sure you want to clear all tasks?")) {
        return;
      }
      try {
        taskService.clearAll();
        refresh();
      } catch (error) {
        alert(error.message);
      }
    });

    window.addEventListener("click", (event) => {
      if (event.target === dom.taskModal) {
        closeTaskModal(dom);
      }
    });
  }

  function init() {
    bind();
    refresh();
  }

  return {
    init,
  };
}
