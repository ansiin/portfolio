"use client";

import { FormEvent, useState } from "react";
import { useTodos } from "@/contexts/TodoContext";

export function TaxonomyForms() {
  const { categories, priorities, loading, createCategory, createPriority, seedDemoData } = useTodos();
  const [categoryName, setCategoryName] = useState("");
  const [priorityName, setPriorityName] = useState("");

  async function addCategory(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!categoryName.trim()) return;
    await createCategory({
      categoryName: categoryName.trim(),
      categorySort: categories.length + 1,
      tag: null
    });
    setCategoryName("");
  }

  async function addPriority(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!priorityName.trim()) return;
    await createPriority({
      priorityName: priorityName.trim(),
      prioritySort: priorities.length + 1,
      syncDt: new Date().toISOString()
    });
    setPriorityName("");
  }

  return (
    <div className="side-section">
      <div className="seed-panel">
        <p>Seed data</p>
        <span>Create starter categories, priorities, and reusable demo tasks.</span>
        <button type="button" onClick={() => void seedDemoData()} disabled={loading}>
          {loading ? "Seeding..." : "Seed demo data"}
        </button>
      </div>

      <form onSubmit={addCategory}>
        <label>
          New category
          <input value={categoryName} onChange={(event) => setCategoryName(event.target.value)} placeholder="School" maxLength={128} />
        </label>
        <button type="submit">Add category</button>
      </form>

      <form onSubmit={addPriority}>
        <label>
          New priority
          <input value={priorityName} onChange={(event) => setPriorityName(event.target.value)} placeholder="High" maxLength={128} />
        </label>
        <button type="submit">Add priority</button>
      </form>

      <div className="taxonomy-list">
        <p>Categories: {categories.length}</p>
        <p>Priorities: {priorities.length}</p>
      </div>
    </div>
  );
}
