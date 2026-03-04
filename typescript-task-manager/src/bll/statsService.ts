import type { Task, TaskStatistics } from "../types/entities";
import { groupBy } from "../shared/utils";

const now = () => new Date();

export function calculateTaskStats(tasks: Task[]): TaskStatistics {
  const byStatusGrouped = groupBy(tasks, (task) => task.status);
  const byPriorityGrouped = groupBy(tasks, (task) => task.priority);
  const completed = tasks.filter((task) => task.status === "completed").length;
  const overdue = tasks.filter((task) => {
    if (!task.dueDate) {
      return false;
    }
    return new Date(task.dueDate) < now() && task.status !== "completed";
  }).length;

  return {
    total: tasks.length,
    completed,
    overdue,
    completionRate: tasks.length === 0 ? 0 : Number(((completed / tasks.length) * 100).toFixed(2)),
    byStatus: {
      todo: byStatusGrouped.todo?.length ?? 0,
      "in-progress": byStatusGrouped["in-progress"]?.length ?? 0,
      completed: byStatusGrouped.completed?.length ?? 0,
      cancelled: byStatusGrouped.cancelled?.length ?? 0,
    },
    byPriority: {
      low: byPriorityGrouped.low?.length ?? 0,
      medium: byPriorityGrouped.medium?.length ?? 0,
      high: byPriorityGrouped.high?.length ?? 0,
      urgent: byPriorityGrouped.urgent?.length ?? 0,
    },
  };
}
