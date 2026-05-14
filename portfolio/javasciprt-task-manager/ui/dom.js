export function bindDom() {
  return {
    taskList: document.getElementById("taskList"),
    taskModal: document.getElementById("taskModal"),
    modalTitle: document.getElementById("modalTitle"),
    closeBtn: document.querySelector(".close"),
    cancelBtn: document.getElementById("cancelBtn"),
    taskForm: document.getElementById("taskForm"),
    addTaskBtn: document.getElementById("addTaskBtn"),
    clearTasksBtn: document.getElementById("clearTasksBtn"),
    searchBtn: document.getElementById("searchBtn"),
    searchInput: document.getElementById("searchInput"),
    applyFiltersBtn: document.getElementById("applyFiltersBtn"),
    clearFiltersBtn: document.getElementById("clearFiltersBtn"),
    statusFilter: document.getElementById("statusFilter"),
    priorityFilter: document.getElementById("priorityFilter"),
    dueDateFilter: document.getElementById("dueDateFilter"),
    tagsFilter: document.getElementById("tagsFilter"),
  };
}
