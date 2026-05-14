<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';

import { getErrorMessage } from '../lib/api';
import { formatDateTime } from '../lib/dates';
import { useCategoriesStore } from '../stores/categories';
import type { TodoCategory } from '../types/todo';

const categoriesStore = useCategoriesStore();

const form = reactive({
  id: '',
  categoryName: '',
  categorySort: 1,
  tag: '',
  syncDt: '',
});

const saveError = ref<string | null>(null);
const loadError = ref<string | null>(null);
const working = ref(false);
const editingId = ref<string | null>(null);

const resetForm = () => {
  editingId.value = null;
  form.id = '';
  form.categoryName = '';
  form.categorySort = categoriesStore.items.length + 1 || 1;
  form.tag = '';
  form.syncDt = '';
  saveError.value = null;
};

const loadCategories = async () => {
  loadError.value = null;

  try {
    await categoriesStore.loadItems();
    if (!editingId.value) {
      form.categorySort = categoriesStore.items.length + 1 || 1;
    }
  } catch (error) {
    loadError.value = getErrorMessage(error, 'Unable to load categories');
  }
};

onMounted(async () => {
  await loadCategories();
  resetForm();
});

const startEditing = (item: TodoCategory) => {
  editingId.value = item.id;
  form.id = item.id;
  form.categoryName = item.categoryName;
  form.categorySort = item.categorySort;
  form.tag = item.tag || '';
  form.syncDt = item.syncDt;
  saveError.value = null;
};

const handleSubmit = async () => {
  working.value = true;
  saveError.value = null;

  try {
    if (editingId.value) {
      await categoriesStore.updateItem({
        id: form.id,
        categoryName: form.categoryName.trim(),
        categorySort: Number(form.categorySort),
        tag: form.tag.trim() || null,
        syncDt: form.syncDt,
      });
    } else {
      await categoriesStore.createItem({
        id: crypto.randomUUID(),
        categoryName: form.categoryName.trim(),
        categorySort: Number(form.categorySort),
        tag: form.tag.trim() || null,
      });
    }

    resetForm();
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to save category');
  } finally {
    working.value = false;
  }
};

const handleDelete = async (item: TodoCategory) => {
  const confirmed = window.confirm(`Delete category "${item.categoryName}"?`);
  if (!confirmed) {
    return;
  }

  try {
    await categoriesStore.deleteItem(item.id);
    if (editingId.value === item.id) {
      resetForm();
    }
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to delete category');
  }
};
</script>

<template>
  <section class="page">
    <header class="page__header">
      <div>
        <p class="page__eyebrow">Configuration</p>
        <h1 class="page__title">Categories</h1>
        <p class="page__subtitle">Create reusable categories first so task forms have valid foreign keys.</p>
      </div>
    </header>

    <p v-if="loadError" class="notice notice--danger">{{ loadError }}</p>

    <div class="panel-grid">
      <section class="panel">
        <div class="panel__header">
          <h2 class="panel__title">{{ editingId ? 'Edit category' : 'Create category' }}</h2>
          <button v-if="editingId" type="button" class="button button--ghost button--small" @click="resetForm">
            Cancel
          </button>
        </div>

        <form class="form-stack" @submit.prevent="handleSubmit">
          <label class="field">
            <span class="field__label">Category name</span>
            <input v-model.trim="form.categoryName" class="field__control" type="text" maxlength="128" required />
          </label>

          <label class="field">
            <span class="field__label">Sort order</span>
            <input v-model.number="form.categorySort" class="field__control" type="number" min="0" required />
          </label>

          <label class="field">
            <span class="field__label">Tag</span>
            <input v-model.trim="form.tag" class="field__control" type="text" maxlength="255" />
          </label>

          <p v-if="saveError" class="notice notice--danger">{{ saveError }}</p>

          <button type="submit" class="button button--primary" :disabled="working">
            {{ working ? 'Saving...' : editingId ? 'Update category' : 'Create category' }}
          </button>
        </form>
      </section>

      <section class="panel">
        <div class="panel__header">
          <h2 class="panel__title">Existing categories</h2>
          <button type="button" class="button button--ghost button--small" :disabled="categoriesStore.loading" @click="loadCategories">
            Reload
          </button>
        </div>

        <div v-if="categoriesStore.items.length === 0" class="empty-state">
          No categories yet. Create the first one on the left.
        </div>

        <ul v-else class="plain-list plain-list--cards">
          <li v-for="item in categoriesStore.items" :key="item.id" class="list-card list-card--stacked">
            <div class="list-card__head">
              <div>
                <strong>{{ item.categoryName }}</strong>
                <p>Sort: {{ item.categorySort }}<span v-if="item.tag"> · Tag: {{ item.tag }}</span></p>
              </div>
              <span class="status-pill status-pill--muted">Sync {{ formatDateTime(item.syncDt) }}</span>
            </div>

            <div class="action-row">
              <button type="button" class="button button--ghost button--small" @click="startEditing(item)">Edit</button>
              <button type="button" class="button button--danger button--small" @click="handleDelete(item)">Delete</button>
            </div>
          </li>
        </ul>
      </section>
    </div>
  </section>
</template>
