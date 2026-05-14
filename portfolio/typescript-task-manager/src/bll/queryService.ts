import type { QueryInput } from "../types/dto";
import type { Task, TaskPriority } from "../types/entities";
import { sortBy } from "../shared/utils";

const PRIORITY_ORDER: Record<TaskPriority, number> = {
  low: 0,
  medium: 1,
  high: 2,
  urgent: 3,
};

export function runTaskQuery(tasks: Task[], query: QueryInput): Task[] {
  let result = [...tasks];

  if (query.search && query.search.trim().length > 0) {
    const q = query.search.toLowerCase();
    result = result.filter((task) => {
      return (
        task.title.toLowerCase().includes(q) ||
        task.description.toLowerCase().includes(q) ||
        task.tags.some((tag) => tag.toLowerCase().includes(q))
      );
    });
  }

  if (query.filter) {
    if (query.filter.status) {
      result = result.filter((task) => task.status === query.filter?.status);
    }
    if (query.filter.priority) {
      result = result.filter((task) => task.priority === query.filter?.priority);
    }
    if (query.filter.categoryId) {
      result = result.filter((task) => task.categoryId === query.filter?.categoryId);
    }
    if (query.filter.dueDate) {
      const filterDate = new Date(query.filter.dueDate).toDateString();
      result = result.filter((task) => {
        if (!task.dueDate) {
          return false;
        }
        return new Date(task.dueDate).toDateString() === filterDate;
      });
    }
    if (query.filter.tag) {
      const tag = query.filter.tag.toLowerCase();
      result = result.filter((task) => task.tags.some((item) => item.toLowerCase() === tag));
    }
  }

  if (query.sort) {
    const { field, direction } = query.sort;
    if (field === "priority") {
      return sortBy(result, (task) => PRIORITY_ORDER[task.priority], direction);
    }
    if (field === "dueDate") {
      return sortBy(
        result,
        (task) => (task.dueDate ? new Date(task.dueDate).getTime() : Number.MAX_SAFE_INTEGER),
        direction,
      );
    }
    return sortBy(result, (task) => String(task[field]), direction);
  }

  return result;
}
