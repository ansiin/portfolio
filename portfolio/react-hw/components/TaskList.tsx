"use client";

import { useMemo, useState } from "react";
import { useTodos } from "@/contexts/TodoContext";
import type { TodoTask } from "@/types/api";

type Filter = "active" | "completed" | "archived" | "all";

function formatDate(value: string | null): string {
  if (!value) return "No due date";
  return new Intl.DateTimeFormat("en-GB", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value));
}

export function TaskList() {
  const { tasks, categories, priorities, updateTask, deleteTask } = useTodos();
  const [filter, setFilter] = useState<Filter>("active");

  const categoryById = useMemo(() => new Map(categories.map((category) => [category.id, category])), [categories]);
  const priorityById = useMemo(() => new Map(priorities.map((priority) => [priority.id, priority])), [priorities]);

  const filteredTasks = tasks
    .filter((task) => {
      if (filter === "active") return !task.isCompleted && !task.isArchived;
      if (filter === "completed") return task.isCompleted && !task.isArchived;
      if (filter === "archived") return task.isArchived;
      return true;
    })
    .sort((a, b) => a.taskSort - b.taskSort);

  function patchTask(task: TodoTask, patch: Partial<TodoTask>) {
    void updateTask({ ...task, ...patch, syncDt: new Date().toISOString() });
  }

  return (
    <section className="task-list" aria-label="Todo tasks">
      <div className="list-toolbar">
        <div className="segmented">
          {(["active", "completed", "archived", "all"] as Filter[]).map((item) => (
            <button key={item} type="button" className={filter === item ? "active" : ""} onClick={() => setFilter(item)}>
              {item}
            </button>
          ))}
        </div>
        <span>{filteredTasks.length} shown</span>
      </div>

      {filteredTasks.length === 0 ? (
        <p className="empty-state">No tasks in this view.</p>
      ) : (
        <ul className="tasks">
          {filteredTasks.map((task) => (
            <li key={task.id} className={task.isCompleted ? "done" : ""}>
              <label className="check-row">
                <input type="checkbox" checked={task.isCompleted} onChange={(event) => patchTask(task, { isCompleted: event.target.checked })} />
                <span>{task.taskName || "Untitled task"}</span>
              </label>

              <div className="task-meta">
                <span>{categoryById.get(task.todoCategoryId)?.categoryName || "No category"}</span>
                <span>{priorityById.get(task.todoPriorityId)?.priorityName || "No priority"}</span>
                <span>{formatDate(task.dueDt)}</span>
              </div>

              <div className="task-actions">
                <button type="button" onClick={() => patchTask(task, { isArchived: !task.isArchived })}>
                  {task.isArchived ? "Restore" : "Archive"}
                </button>
                <button type="button" className="danger" onClick={() => void deleteTask(task.id)}>
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
