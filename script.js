/**
 * Task Management System - Vanilla JavaScript Implementation
 * Simple task management with local storage persistence
 */

// ==========================================
// CONFIGURATION
// ==========================================
const VALID_STATUSES = ["todo", "in-progress", "completed", "cancelled"];
const VALID_PRIORITIES = ["low", "medium", "high", "urgent"];
const STORAGE_KEY = "tasks";

// ==========================================
// DOM CACHING
// ==========================================
const taskList = document.getElementById("taskList");
const taskModal = document.getElementById("taskModal");
const modalTitle = document.getElementById("modalTitle");
const closeBtn = document.querySelector(".close");
const cancelBtn = document.getElementById("cancelBtn");
const taskForm = document.getElementById("taskForm");
const addTaskBtn = document.getElementById("addTaskBtn");
const clearTasksBtn = document.getElementById("clearTasksBtn");
const searchBtn = document.getElementById("searchBtn");
const searchInput = document.getElementById("searchInput");
const applyFiltersBtn = document.getElementById("applyFiltersBtn");
const clearFiltersBtn = document.getElementById("clearFiltersBtn");

console.log("DOM elements cached");

// ==========================================
// HTML TEMPLATES (Separated from logic)
// ==========================================
const templates = {
  /**
   * Empty state template when no tasks exist
   */
  emptyState: `
    <div class="empty-state">
      <h3>No tasks found</h3>
      <p>Click "Add New Task" to create your first task</p>
    </div>
  `,

  /**
   * Single task card template
   * @param {Object} task - Task object to render
   * @returns {string} HTML string for task card
   */
  taskCard: function (task) {
    return `
      <div class="task-card">
        <div class="task-header">
          <div>
            <div class="task-id">${task.id}</div>
            <h3 class="task-title">${task.title}</h3>
          </div>
        </div>
        <div class="task-details">
          <div><span class="status-${task.status}">${task.status}</span></div>
          <div><span class="priority-${task.priority}">${task.priority}</span></div>
          ${task.dueDate ? `<div>Due: ${new Date(task.dueDate).toLocaleDateString()}</div>` : ""}
        </div>
        ${task.description ? `<div class="task-description">${task.description}</div>` : ""}
        ${task.tags.length ? `<div class="task-tags">${task.tags.map((tag) => `<span class="tag">${tag}</span>`).join("")}</div>` : ""}
        <div class="task-actions">
          <button onclick="editTask('${task.id}')">Edit</button>
          <button onclick="deleteTask('${task.id}')">Delete</button>
        </div>
      </div>
    `;
  },
};

// ==========================================
// CORE FUNCTIONS
// ==========================================

function generateId() {
  return Date.now().toString(36) + Math.random().toString(36).substr(2);
}

function createTask(taskData) {
  const now = new Date().toISOString();
  return {
    id: taskData.id || generateId(),
    title: taskData.title,
    description: taskData.description || "",
    status: taskData.status || "todo",
    priority: taskData.priority || "medium",
    dueDate: taskData.dueDate || null,
    tags: taskData.tags || [],
    createdAt: now,
    updatedAt: now,
  };
}

function updateTask(existingTask, updates) {
  return { ...existingTask, ...updates, updatedAt: new Date().toISOString() };
}

// ==========================================
// VALIDATION
// ==========================================
const validators = {
  title: function (title) {
    if (!title || typeof title !== "string" || title.trim().length === 0) {
      throw new Error("Title is required");
    }
    if (title.length > 200) {
      throw new Error("Title cannot exceed 200 characters");
    }
    return true;
  },
  description: function (description) {
    if (description && typeof description !== "string") {
      throw new Error("Description must be a string");
    }
    if (description && description.length > 1000) {
      throw new Error("Description cannot exceed 1000 characters");
    }
    return true;
  },
  status: function (status) {
    if (
      !status ||
      typeof status !== "string" ||
      !VALID_STATUSES.includes(status.toLowerCase())
    ) {
      throw new Error(`Status must be one of: ${VALID_STATUSES.join(", ")}`);
    }
    return true;
  },
  priority: function (priority) {
    if (
      !priority ||
      typeof priority !== "string" ||
      !VALID_PRIORITIES.includes(priority.toLowerCase())
    ) {
      throw new Error(
        `Priority must be one of: ${VALID_PRIORITIES.join(", ")}`,
      );
    }
    return true;
  },
  dueDate: function (dueDate) {
    if (dueDate && isNaN(new Date(dueDate).getTime())) {
      throw new Error("Due date must be a valid date");
    }
    return true;
  },
  tags: function (tags) {
    if (tags && !Array.isArray(tags)) {
      throw new Error("Tags must be an array");
    }
    tags?.forEach((tag, index) => {
      if (typeof tag !== "string") {
        throw new Error(`Tag at index ${index} must be a string`);
      }
      if (!tag.trim()) {
        throw new Error(`Tag at index ${index} cannot be empty`);
      }
      if (tag.length > 50) {
        throw new Error(`Tag at index ${index} cannot exceed 50 characters`);
      }
    });
    return true;
  },
  validate: function (taskData) {
    const errors = [];
    Object.entries(taskData).forEach(([key, value]) => {
      if (validators[key]) {
        try {
          validators[key](value);
        } catch (error) {
          errors.push(error.message);
        }
      }
    });
    if (errors.length) {
      throw new Error(errors.join("; "));
    }
    return true;
  },
};

// ==========================================
// STORAGE
// ==========================================
function initStorage() {
  if (!localStorage.getItem(STORAGE_KEY)) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify([]));
  }
}

function getTasks() {
  initStorage();
  return JSON.parse(localStorage.getItem(STORAGE_KEY) || "[]");
}

function getTask(taskId) {
  const task = getTasks().find((task) => task.id === taskId);
  if (!task) {
    throw new Error(`Task with ID ${taskId} not found`);
  }
  return task;
}

function saveTask(task) {
  const tasks = getTasks();
  const index = tasks.findIndex((existingTask) => existingTask.id === task.id);
  if (index !== -1) {
    tasks[index] = task;
  } else {
    tasks.push(task);
  }
  localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
  return task;
}

function removeTask(taskId) {
  const tasks = getTasks().filter((task) => task.id !== taskId);
  localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
  return true;
}

function clearAllTasks() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify([]));
  return true;
}

// ==========================================
// TASK MANAGEMENT
// ==========================================
function addTask(taskData) {
  validators.validate(taskData);
  return saveTask(createTask(taskData));
}

function updateExistingTask(taskId, taskData) {
  validators.validate(taskData);
  const existingTask = getTask(taskId);
  return saveTask(updateTask(existingTask, taskData));
}

function deleteTask(taskId) {
  return removeTask(taskId);
}

function listTasks() {
  return getTasks();
}

function filterTasks(filterCriteria) {
  return getTasks().filter((task) => {
    return Object.entries(filterCriteria).every(([key, value]) => {
      if (key === "tags") {
        return value.every((tag) => task.tags.includes(tag));
      }
      if (key === "dueDate") {
        if (!task.dueDate) {
          return false;
        }
        const taskDate = new Date(task.dueDate).toDateString();
        const filterDate = new Date(value).toDateString();
        return taskDate === filterDate;
      }
      if (key === "minPriority") {
        const priorityOrder = ["low", "medium", "high", "urgent"];
        return (
          priorityOrder.indexOf(task.priority) >= priorityOrder.indexOf(value)
        );
      }
      return task[key] === value;
    });
  });
}

function searchTasks(searchQuery) {
  const searchTerm = searchQuery.toLowerCase();
  return getTasks().filter((task) => {
    return (
      task.title.toLowerCase().includes(searchTerm) ||
      task.description.toLowerCase().includes(searchTerm) ||
      task.tags.some((tag) => tag.toLowerCase().includes(searchTerm)) ||
      task.status.toLowerCase().includes(searchTerm) ||
      task.priority.toLowerCase().includes(searchTerm)
    );
  });
}

// ==========================================
// DOM RENDERING
// ==========================================
function renderTasks(tasksToRender = null) {
  const tasks = tasksToRender || listTasks();

  if (!tasks.length) {
    taskList.innerHTML = templates.emptyState;
    return;
  }

  taskList.innerHTML = tasks.map(templates.taskCard).join("");
}

function openModal(modalTitleText = "Add New Task", taskToEdit = null) {
  modalTitle.textContent = modalTitleText;
  if (taskToEdit) {
    document.getElementById("taskId").value = taskToEdit.id;
    document.getElementById("taskTitle").value = taskToEdit.title;
    document.getElementById("taskDescription").value =
      taskToEdit.description || "";
    document.getElementById("taskStatus").value = taskToEdit.status;
    document.getElementById("taskPriority").value = taskToEdit.priority;
    document.getElementById("taskDueDate").value =
      taskToEdit.dueDate?.split("T")[0] || "";
    document.getElementById("taskTags").value = taskToEdit.tags.join(", ");
  } else {
    taskForm.reset();
  }
  taskModal.style.display = "block";
}

function closeModal() {
  taskModal.style.display = "none";
  taskForm.reset();
}

// ==========================================
// EVENT HANDLERS
// ==========================================
function editTask(taskId) {
  try {
    openModal("Edit Task", getTask(taskId));
  } catch (error) {
    alert(error.message);
  }
}

taskForm.addEventListener("submit", function (event) {
  event.preventDefault();
  try {
    const taskId = document.getElementById("taskId").value;
    const tagsInput = document.getElementById("taskTags").value;
    const tags = tagsInput
      .split(",")
      .map((tag) => tag.trim())
      .filter((tag) => tag);
    const taskData = {
      title: document.getElementById("taskTitle").value,
      description: document.getElementById("taskDescription").value,
      status: document.getElementById("taskStatus").value,
      priority: document.getElementById("taskPriority").value,
      dueDate: document.getElementById("taskDueDate").value || null,
      tags,
    };
    if (taskId) {
      updateExistingTask(taskId, taskData);
    } else {
      addTask(taskData);
    }
    closeModal();
    renderTasks();
  } catch (error) {
    alert(error.message);
  }
});

searchBtn.addEventListener("click", function () {
  const searchQuery = searchInput.value.trim();
  if (!searchQuery) {
    renderTasks();
    return;
  }
  try {
    renderTasks(searchTasks(searchQuery));
  } catch (error) {
    alert(error.message);
  }
});

applyFiltersBtn.addEventListener("click", function () {
  const filters = {};
  const statusFilter = document.getElementById("statusFilter").value;
  if (statusFilter) {
    filters.status = statusFilter;
  }
  const priorityFilter = document.getElementById("priorityFilter").value;
  if (priorityFilter) {
    filters.priority = priorityFilter;
  }
  const dueDateFilter = document.getElementById("dueDateFilter").value;
  if (dueDateFilter) {
    filters.dueDate = dueDateFilter;
  }
  const tagsInput = document.getElementById("tagsFilter").value;
  if (tagsInput) {
    filters.tags = tagsInput
      .split(",")
      .map((tag) => tag.trim())
      .filter((tag) => tag);
  }
  try {
    renderTasks(filterTasks(filters));
  } catch (error) {
    alert(error.message);
  }
});

clearFiltersBtn.addEventListener("click", function () {
  document.getElementById("statusFilter").value = "";
  document.getElementById("priorityFilter").value = "";
  document.getElementById("dueDateFilter").value = "";
  document.getElementById("tagsFilter").value = "";
  renderTasks();
});

clearTasksBtn.addEventListener("click", function () {
  if (!confirm("Are you sure you want to clear all tasks?")) {
    return;
  }
  try {
    clearAllTasks();
    renderTasks();
  } catch (error) {
    alert(error.message);
  }
});

addTaskBtn.addEventListener("click", function () {
  openModal();
});

cancelBtn.addEventListener("click", closeModal);
closeBtn.addEventListener("click", closeModal);

window.addEventListener("click", function (event) {
  if (event.target === taskModal) {
    closeModal();
  }
});

// Initial render
renderTasks();
