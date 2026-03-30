import { ref } from 'vue';
import { defineStore } from 'pinia';

import { apiRequest } from '../lib/api';
import type { TodoPriority, TodoPriorityPayload } from '../types/todo';

const sortPriorities = (items: TodoPriority[]): TodoPriority[] =>
  [...items].sort(
    (left, right) =>
      left.prioritySort - right.prioritySort || left.priorityName.localeCompare(right.priorityName, undefined, { sensitivity: 'base' }),
  );

export const usePrioritiesStore = defineStore('priorities', () => {
  const items = ref<TodoPriority[]>([]);
  const loaded = ref(false);
  const loading = ref(false);

  const upsert = (item: TodoPriority) => {
    const index = items.value.findIndex((existing) => existing.id === item.id);

    if (index === -1) {
      items.value = sortPriorities([...items.value, item]);
      return;
    }

    const nextItems = [...items.value];
    nextItems.splice(index, 1, item);
    items.value = sortPriorities(nextItems);
  };

  const loadItems = async (force = false) => {
    if (loaded.value && !force) {
      return items.value;
    }

    loading.value = true;

    try {
      const response = await apiRequest<TodoPriority[]>('/TodoPriorities');
      items.value = sortPriorities(response);
      loaded.value = true;
      return items.value;
    } finally {
      loading.value = false;
    }
  };

  const createItem = async (payload: TodoPriorityPayload) => {
    const created = await apiRequest<TodoPriority>('/TodoPriorities', {
      method: 'POST',
      body: payload,
    });

    upsert(created);
    return created;
  };

  const updateItem = async (payload: TodoPriorityPayload) => {
    const updated = await apiRequest<TodoPriority>(`/TodoPriorities/${payload.id}`, {
      method: 'PUT',
      body: payload,
    });

    upsert(updated);
    return updated;
  };

  const deleteItem = async (id: string) => {
    await apiRequest<void>(`/TodoPriorities/${id}`, {
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
