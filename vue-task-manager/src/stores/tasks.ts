import { ref } from 'vue';
import { defineStore } from 'pinia';

import { apiRequest } from '../lib/api';
import type { TodoTask, TodoTaskPayload } from '../types/todo';

const sortTasks = (items: TodoTask[]): TodoTask[] =>
  [...items].sort((left, right) => {
    const leftCreated = new Date(left.createdDt).getTime();
    const rightCreated = new Date(right.createdDt).getTime();

    return left.taskSort - right.taskSort || leftCreated - rightCreated;
  });

export const useTasksStore = defineStore('tasks', () => {
  const items = ref<TodoTask[]>([]);
  const loaded = ref(false);
  const loading = ref(false);

  const upsert = (item: TodoTask) => {
    const index = items.value.findIndex((existing) => existing.id === item.id);

    if (index === -1) {
      items.value = sortTasks([...items.value, item]);
      return;
    }

    const nextItems = [...items.value];
    nextItems.splice(index, 1, item);
    items.value = sortTasks(nextItems);
  };

  const loadItems = async (force = false) => {
    if (loaded.value && !force) {
      return items.value;
    }

    loading.value = true;

    try {
      const response = await apiRequest<TodoTask[]>('/TodoTasks');
      items.value = sortTasks(response);
      loaded.value = true;
      return items.value;
    } finally {
      loading.value = false;
    }
  };

  const createItem = async (payload: TodoTaskPayload) => {
    const created = await apiRequest<TodoTask>('/TodoTasks', {
      method: 'POST',
      body: payload,
    });

    upsert(created);
    return created;
  };

  const updateItem = async (payload: TodoTaskPayload) => {
    const updated = await apiRequest<TodoTask>(`/TodoTasks/${payload.id}`, {
      method: 'PUT',
      body: payload,
    });

    upsert(updated);
    return updated;
  };

  const deleteItem = async (id: string) => {
    await apiRequest<void>(`/TodoTasks/${id}`, {
      method: 'DELETE',
    });

    items.value = items.value.filter((item) => item.id !== id);
  };

  return {
    items,
    loaded,
    loading,
    loadItems,
    createItem,
    updateItem,
    deleteItem,
  };
});
