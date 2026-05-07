"use client";

import { FormEvent, useMemo, useState } from "react";
import { useTodos } from "@/contexts/TodoContext";

export function TaskForm() {
  const { tasks, categories, priorities, createTask } = useTodos();
  const [taskName, setTaskName] = useState("");
  const [dueDt, setDueDt] = useState("");
  const [todoCategoryId, setTodoCategoryId] = useState("");
  const [todoPriorityId, setTodoPriorityId] = useState("");

  const activeCategoryId = useMemo(() => todoCategoryId || categories[0]?.id || "", [categories, todoCategoryId]);
  const activePriorityId = useMemo(() => todoPriorityId || priorities[0]?.id || "", [priorities, todoPriorityId]);
  const canCreate = categories.length > 0 && priorities.length > 0;

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!taskName.trim() || !activeCategoryId || !activePriorityId) return;

    await createTask({
      taskName: taskName.trim(),
      taskSort: Math.max(0, ...tasks.map((task) => task.taskSort)) + 10,
      createdDt: new Date().toISOString(),
      dueDt: dueDt ? new Date(dueDt).toISOString() : null,
      isCompleted: false,
      isArchived: false,
      todoCategoryId: activeCategoryId,
      todoPriorityId: activePriorityId
    });

    setTaskName("");
    setDueDt("");
  }

  return (
    <form className="task-form" onSubmit={onSubmit}>
      <label>
        Task
        <input value={taskName} onChange={(event) => setTaskName(event.target.value)} placeholder="Write task name" maxLength={128} required />
      </label>

      <div className="form-row">
        <label>
          Category
          <select value={activeCategoryId} onChange={(event) => setTodoCategoryId(event.target.value)} disabled={!canCreate}>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.categoryName || "Untitled category"}
              </option>
            ))}
          </select>
        </label>

        <label>
          Priority
          <select value={activePriorityId} onChange={(event) => setTodoPriorityId(event.target.value)} disabled={!canCreate}>
            {priorities.map((priority) => (
              <option key={priority.id} value={priority.id}>
                {priority.priorityName || "Untitled priority"}
              </option>
            ))}
          </select>
        </label>

        <label>
          Due
          <input type="datetime-local" value={dueDt} onChange={(event) => setDueDt(event.target.value)} />
        </label>
      </div>

      {!canCreate && <p className="hint">Create at least one category and one priority before adding tasks.</p>}

      <button className="primary-button" type="submit" disabled={!canCreate}>
        Add task
      </button>
    </form>
  );
}
