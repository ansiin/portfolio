const emptyState = `
  <div class="empty-state">
    <h3>No tasks found</h3>
    <p>Click "Add New Task" to create your first task</p>
  </div>
`;

function renderTaskCard(task) {
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
        <button data-action="edit" data-id="${task.id}">Edit</button>
        <button data-action="delete" data-id="${task.id}">Delete</button>
      </div>
    </div>
  `;
}

export function renderTaskList(root, tasks) {
  if (!tasks.length) {
    root.innerHTML = emptyState;
    return;
  }
  root.innerHTML = tasks.map(renderTaskCard).join("");
}
