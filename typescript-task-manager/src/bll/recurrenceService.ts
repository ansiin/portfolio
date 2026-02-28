import type { IRecurrenceService } from "../api/bll/IRecurrenceService";
import type { Task } from "../types/entities";
import { generateId } from "../shared/utils";

function addDays(date: Date, amount: number): Date {
  const copy = new Date(date);
  copy.setDate(copy.getDate() + amount);
  return copy;
}

function addMonths(date: Date, amount: number): Date {
  const copy = new Date(date);
  copy.setMonth(copy.getMonth() + amount);
  return copy;
}

export class RecurrenceService implements IRecurrenceService {
  createNext(task: Task): Task | null {
    if (!task.dueDate || task.recurrence.type === "none") {
      return null;
    }

    const baseDate = new Date(task.dueDate);
    let nextDate: Date;
    if (task.recurrence.type === "daily") {
      nextDate = addDays(baseDate, task.recurrence.interval);
    } else if (task.recurrence.type === "weekly") {
      nextDate = addDays(baseDate, task.recurrence.interval * 7);
    } else {
      nextDate = addMonths(baseDate, task.recurrence.interval);
    }

    if (task.recurrence.endDate && nextDate > new Date(task.recurrence.endDate)) {
      return null;
    }

    const now = new Date().toISOString();
    return {
      ...task,
      id: generateId(),
      status: "todo",
      dueDate: nextDate.toISOString(),
      createdAt: now,
      updatedAt: now,
    };
  }
}
