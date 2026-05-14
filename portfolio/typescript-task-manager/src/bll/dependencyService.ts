import type { Task } from "../types/entities";

export function hasDependencyCycle(tasks: Task[], taskId: string, dependencyIds: string[]): boolean {
  const graph = new Map<string, string[]>();
  for (const task of tasks) {
    graph.set(task.id, task.dependencyIds);
  }
  graph.set(taskId, dependencyIds);

  const visiting = new Set<string>();
  const visited = new Set<string>();

  const dfs = (id: string): boolean => {
    if (visiting.has(id)) {
      return true;
    }
    if (visited.has(id)) {
      return false;
    }
    visiting.add(id);
    const deps = graph.get(id) ?? [];
    for (const depId of deps) {
      if (dfs(depId)) {
        return true;
      }
    }
    visiting.delete(id);
    visited.add(id);
    return false;
  };

  return dfs(taskId);
}

export function canMarkTaskCompleted(tasks: Task[], task: Task): boolean {
  return task.dependencyIds.every((depId) => {
    const dep = tasks.find((item) => item.id === depId);
    return dep?.status === "completed";
  });
}
