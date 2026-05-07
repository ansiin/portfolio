"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { TaskForm } from "@/components/TaskForm";
import { TaskList } from "@/components/TaskList";
import { TaxonomyForms } from "@/components/TaxonomyForms";
import { useAuth } from "@/contexts/AuthContext";
import { useTodos } from "@/contexts/TodoContext";

export default function DashboardPage() {
  const { session, loading: authLoading, logout } = useAuth();
  const { reload, loading, error, tasks } = useTodos();
  const router = useRouter();

  useEffect(() => {
    if (!authLoading && !session) router.replace("/");
  }, [authLoading, router, session]);

  useEffect(() => {
    if (session) void reload();
  }, [reload, session]);

  if (authLoading || !session) return <main className="loading-screen">Loading workspace...</main>;

  const completed = tasks.filter((task) => task.isCompleted && !task.isArchived).length;
  const active = tasks.filter((task) => !task.isCompleted && !task.isArchived).length;

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div>
          <p className="product-name">TalTech ToDo</p>
          <p className="signed-in">
            {session.firstName || "User"} {session.lastName || ""}
          </p>
        </div>

        <nav aria-label="Workspace summary">
          <a href="#tasks">Tasks</a>
          <a href="#settings">Lists</a>
        </nav>

        <div className="summary">
          <span>{active} active</span>
          <span>{completed} completed</span>
        </div>

        <button type="button" onClick={logout}>
          Sign out
        </button>
      </aside>

      <section className="workspace">
        <header className="workspace-header">
          <div>
            <h1>ToDo tasks</h1>
            <p>Data is loaded from https://taltech.akaver.com/api/v1 with JWT authorization.</p>
          </div>
          <button type="button" onClick={() => void reload()} disabled={loading}>
            {loading ? "Refreshing..." : "Refresh"}
          </button>
        </header>

        {error && <p className="form-error">{error}</p>}

        <div className="content-grid">
          <section id="tasks" className="main-column">
            <TaskForm />
            <TaskList />
          </section>

          <aside id="settings" className="right-column">
            <TaxonomyForms />
          </aside>
        </div>
      </section>
    </main>
  );
}
