"use client";

import { createContext, useCallback, useContext, useMemo, useReducer, type ReactNode } from "react";
import { todoApi } from "@/lib/api-client";
import type { TodoCategory, TodoCategoryCreate, TodoPriority, TodoPriorityCreate, TodoTask, TodoTaskCreate } from "@/types/api";

type TodoState = {
  tasks: TodoTask[];
  categories: TodoCategory[];
  priorities: TodoPriority[];
  loading: boolean;
  error: string | null;
};

type TodoAction =
  | { type: "start" }
  | { type: "loaded"; tasks: TodoTask[]; categories: TodoCategory[]; priorities: TodoPriority[] }
  | { type: "task_added"; task: TodoTask }
  | { type: "task_updated"; task: TodoTask }
  | { type: "task_removed"; id: string }
  | { type: "category_added"; category: TodoCategory }
  | { type: "priority_added"; priority: TodoPriority }
  | { type: "error"; error: string };

type TodoContextValue = TodoState & {
  reload: () => Promise<void>;
  seedDemoData: () => Promise<void>;
  createTask: (payload: TodoTaskCreate) => Promise<void>;
  updateTask: (task: TodoTask) => Promise<void>;
  deleteTask: (id: string) => Promise<void>;
  createCategory: (payload: TodoCategoryCreate) => Promise<void>;
  createPriority: (payload: TodoPriorityCreate) => Promise<void>;
};

const TodoContext = createContext<TodoContextValue | null>(null);

function todoReducer(state: TodoState, action: TodoAction): TodoState {
  switch (action.type) {
    case "start":
      return { ...state, loading: true, error: null };
    case "loaded":
      return {
        tasks: action.tasks,
        categories: action.categories,
        priorities: action.priorities,
        loading: false,
        error: null
      };
    case "task_added":
      return { ...state, tasks: [action.task, ...state.tasks], error: null };
    case "task_updated":
      return { ...state, tasks: state.tasks.map((task) => (task.id === action.task.id ? action.task : task)), error: null };
    case "task_removed":
      return { ...state, tasks: state.tasks.filter((task) => task.id !== action.id), error: null };
    case "category_added":
      return { ...state, categories: [...state.categories, action.category], error: null };
    case "priority_added":
      return { ...state, priorities: [...state.priorities, action.priority], error: null };
    case "error":
      return { ...state, loading: false, error: action.error };
  }
}

function toErrorMessage(error: unknown): string {
  return error instanceof Error ? error.message : "Request failed.";
}

const seedCategories: TodoCategoryCreate[] = [
  { categoryName: "School", categorySort: 10, tag: "seed" },
  { categoryName: "Personal", categorySort: 20, tag: "seed" },
  { categoryName: "Work", categorySort: 30, tag: "seed" }
];

const seedPriorities: TodoPriorityCreate[] = [
  { priorityName: "High", prioritySort: 10, syncDt: "" },
  { priorityName: "Normal", prioritySort: 20, syncDt: "" },
  { priorityName: "Low", prioritySort: 30, syncDt: "" }
];

const seedTaskNames = [
  "Read the Swagger contract",
  "Implement JWT refresh handling",
  "Prepare Docker deployment",
  "Add README public URL"
];

export function TodoProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(todoReducer, {
    tasks: [],
    categories: [],
    priorities: [],
    loading: false,
    error: null
  });

  const reload = useCallback(async () => {
    dispatch({ type: "start" });
    try {
      const [tasks, categories, priorities] = await Promise.all([
        todoApi.tasks.list(),
        todoApi.categories.list(),
        todoApi.priorities.list()
      ]);
      dispatch({ type: "loaded", tasks, categories, priorities });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const seedDemoData = useCallback(async () => {
    dispatch({ type: "start" });

    try {
      const [currentTasks, currentCategories, currentPriorities] = await Promise.all([
        todoApi.tasks.list(),
        todoApi.categories.list(),
        todoApi.priorities.list()
      ]);

      const categories = [...currentCategories];
      for (const seed of seedCategories) {
        const existing = categories.find((category) => category.categoryName?.toLowerCase() === seed.categoryName?.toLowerCase());
        if (!existing) {
          categories.push(await todoApi.categories.create(seed));
        }
      }

      const priorities = [...currentPriorities];
      for (const seed of seedPriorities) {
        const existing = priorities.find((priority) => priority.priorityName?.toLowerCase() === seed.priorityName?.toLowerCase());
        if (!existing) {
          priorities.push(await todoApi.priorities.create({ ...seed, syncDt: new Date().toISOString() }));
        }
      }

      const schoolCategory = categories.find((category) => category.categoryName === "School") ?? categories[0];
      const normalPriority = priorities.find((priority) => priority.priorityName === "Normal") ?? priorities[0];

      const tasks = [...currentTasks];
      if (schoolCategory && normalPriority) {
        for (const [index, taskName] of seedTaskNames.entries()) {
          const exists = tasks.some((task) => task.taskName?.toLowerCase() === taskName.toLowerCase());
          if (!exists) {
            tasks.push(
              await todoApi.tasks.create({
                taskName,
                taskSort: (index + 1) * 10,
                createdDt: new Date().toISOString(),
                dueDt: new Date(Date.now() + (index + 1) * 24 * 60 * 60 * 1000).toISOString(),
                isCompleted: index === 0,
                isArchived: false,
                todoCategoryId: schoolCategory.id,
                todoPriorityId: normalPriority.id
              })
            );
          }
        }
      }

      dispatch({ type: "loaded", tasks, categories, priorities });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const createTask = useCallback(async (payload: TodoTaskCreate) => {
    try {
      dispatch({ type: "task_added", task: await todoApi.tasks.create(payload) });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const updateTask = useCallback(async (task: TodoTask) => {
    try {
      dispatch({ type: "task_updated", task: await todoApi.tasks.update({ ...task, syncDt: new Date().toISOString() }) });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const deleteTask = useCallback(async (id: string) => {
    try {
      await todoApi.tasks.remove(id);
      dispatch({ type: "task_removed", id });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const createCategory = useCallback(async (payload: TodoCategoryCreate) => {
    try {
      dispatch({ type: "category_added", category: await todoApi.categories.create(payload) });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const createPriority = useCallback(async (payload: TodoPriorityCreate) => {
    try {
      dispatch({ type: "priority_added", priority: await todoApi.priorities.create(payload) });
    } catch (error) {
      dispatch({ type: "error", error: toErrorMessage(error) });
    }
  }, []);

  const value = useMemo<TodoContextValue>(
    () => ({ ...state, reload, seedDemoData, createTask, updateTask, deleteTask, createCategory, createPriority }),
    [state, reload, seedDemoData, createTask, updateTask, deleteTask, createCategory, createPriority]
  );

  return <TodoContext.Provider value={value}>{children}</TodoContext.Provider>;
}

export function useTodos() {
  const context = useContext(TodoContext);
  if (!context) throw new Error("useTodos must be used inside TodoProvider.");
  return context;
}
