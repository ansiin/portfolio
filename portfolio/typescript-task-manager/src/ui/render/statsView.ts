import type { TaskStatistics } from "../../types/entities";

export function renderStats(root: HTMLElement, stats: TaskStatistics): void {
  root.innerHTML = `
    <div class="stats-grid">
      <div><strong>Total:</strong> ${stats.total}</div>
      <div><strong>Completed:</strong> ${stats.completed}</div>
      <div><strong>Overdue:</strong> ${stats.overdue}</div>
      <div><strong>Completion:</strong> ${stats.completionRate}%</div>
    </div>
  `;
}
