import { ref } from 'vue';
import { defineStore } from 'pinia';

import { apiRequest } from '../lib/api';
import type { TodoCategory, TodoCategoryCreatePayload, TodoCategoryUpdatePayload } from '../types/todo';

const sortCategories = (items: TodoCategory[]): TodoCategory[] =>
  [...items].sort(
    (left, right) =>
      left.categorySort - right.categorySort || left.categoryName.localeCompare(right.categoryName, undefined, { sensitivity: 'base' }),
  );

export const useCategoriesStore = defineStore('categories', () => {
  const items = ref<TodoCategory[]>([]);
  const loaded = ref(false);
  const loading = ref(false);

  const upsert = (item: TodoCategory) => {
    const index = items.value.findIndex((existing) => existing.id === item.id);

    if (index === -1) {
      items.value = sortCategories([...items.value, item]);
      return;
    }

    const nextItems = [...items.value];
    nextItems.splice(index, 1, item);
    items.value = sortCategories(nextItems);
  };

  const loadItems = async (force = false) => {
    if (loaded.value && !force) {
      return items.value;
    }

    loading.value = true;

    try {
      const response = await apiRequest<TodoCategory[]>('/TodoCategories');
      items.value = sortCategories(response);
      loaded.value = true;
      return items.value;
    } finally {
      loading.value = false;
    }
  };

  const createItem = async (payload: TodoCategoryCreatePayload) => {
    const created = await apiRequest<TodoCategory>('/TodoCategories', {
      method: 'POST',
      body: payload,
    });

    upsert(created);
    return created;
  };

  const updateItem = async (payload: TodoCategoryUpdatePayload) => {
    const updated = await apiRequest<TodoCategory>(`/TodoCategories/${payload.id}`, {
      method: 'PUT',
      body: payload,
    });

    upsert(updated);
    return updated;
  };

  const deleteItem = async (id: string) => {
    await apiRequest<void>(`/TodoCategories/${id}`, {
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
