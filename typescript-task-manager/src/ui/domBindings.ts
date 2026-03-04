export type AppDom = {
  taskList: HTMLElement;
  statsBox: HTMLElement;
  taskModal: HTMLElement;
  modalTitle: HTMLElement;
  taskForm: HTMLFormElement;
  addTaskBtn: HTMLButtonElement;
  clearTasksBtn: HTMLButtonElement;
  cancelBtn: HTMLButtonElement;
  closeBtn: HTMLElement;
  searchInput: HTMLInputElement;
  searchBtn: HTMLButtonElement;
  applyFiltersBtn: HTMLButtonElement;
  clearFiltersBtn: HTMLButtonElement;
  statusFilter: HTMLSelectElement;
  priorityFilter: HTMLSelectElement;
  categoryFilter: HTMLSelectElement;
  dueDateFilter: HTMLInputElement;
  tagsFilter: HTMLInputElement;
  sortField: HTMLSelectElement;
  sortDirection: HTMLSelectElement;
  taskDependencies: HTMLSelectElement;
};

function byId<T extends HTMLElement>(id: string): T {
  const element = document.getElementById(id);
  if (!element) {
    throw new Error(`Missing DOM element: ${id}`);
  }
  return element as T;
}

function bySelector<T extends Element>(selector: string): T {
  const element = document.querySelector(selector);
  if (!element) {
    throw new Error(`Missing DOM element: ${selector}`);
  }
  return element as T;
}

export function bindDom(): AppDom {
  return {
    taskList: byId("taskList"),
    statsBox: byId("statsBox"),
    taskModal: byId("taskModal"),
    modalTitle: byId("modalTitle"),
    taskForm: byId("taskForm"),
    addTaskBtn: byId("addTaskBtn"),
    clearTasksBtn: byId("clearTasksBtn"),
    cancelBtn: byId("cancelBtn"),
    closeBtn: bySelector<HTMLElement>(".close"),
    searchInput: byId("searchInput"),
    searchBtn: byId("searchBtn"),
    applyFiltersBtn: byId("applyFiltersBtn"),
    clearFiltersBtn: byId("clearFiltersBtn"),
    statusFilter: byId("statusFilter"),
    priorityFilter: byId("priorityFilter"),
    categoryFilter: byId("categoryFilter"),
    dueDateFilter: byId("dueDateFilter"),
    tagsFilter: byId("tagsFilter"),
    sortField: byId("sortField"),
    sortDirection: byId("sortDirection"),
    taskDependencies: byId("taskDependencies"),
  };
}
