<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';

import { getErrorMessage } from '../lib/api';
import { formatDateTime } from '../lib/dates';
import { usePrioritiesStore } from '../stores/priorities';
import type { TodoPriority } from '../types/todo';

const prioritiesStore = usePrioritiesStore();

const form = reactive({
  id: '',
  priorityName: '',
  prioritySort: 1,
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
  form.priorityName = '';
  form.prioritySort = prioritiesStore.items.length + 1 || 1;
  form.tag = '';
  form.syncDt = new Date().toISOString();
  saveError.value = null;
};

const loadPriorities = async () => {
  loadError.value = null;

  try {
    await prioritiesStore.loadItems();
    if (!editingId.value) {
      form.prioritySort = prioritiesStore.items.length + 1 || 1;
    }
  } catch (error) {
    loadError.value = getErrorMessage(error, 'Unable to load priorities');
  }
};

onMounted(async () => {
  await loadPriorities();
  resetForm();
});

const startEditing = (item: TodoPriority) => {
  editingId.value = item.id;
  form.id = item.id;
  form.priorityName = item.priorityName;
  form.prioritySort = item.prioritySort;
  form.tag = item.tag || '';
  form.syncDt = item.syncDt;
  saveError.value = null;
};

const handleSubmit = async () => {
  working.value = true;
  saveError.value = null;

  try {
    const payload = {
      id: editingId.value || crypto.randomUUID(),
      priorityName: form.priorityName.trim(),
      prioritySort: Number(form.prioritySort),
      tag: form.tag.trim() || null,
      syncDt: form.syncDt || new Date().toISOString(),
    };

    if (editingId.value) {
      await prioritiesStore.updateItem(payload);
    } else {
      await prioritiesStore.createItem(payload);
    }

    resetForm();
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to save priority');
  } finally {
    working.value = false;
  }
};

const handleDelete = async (item: TodoPriority) => {
  const confirmed = window.confirm(`Delete priority "${item.priorityName}"?`);
  if (!confirmed) {
    return;
  }

  try {
    await prioritiesStore.deleteItem(item.id);
    if (editingId.value === item.id) {
      resetForm();
    }
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to delete priority');
  }
};
</script>

<template>
  <section class="page">
    <header class="page__header">
      <div>
        <p class="page__eyebrow">Configuration</p>
        <h1 class="page__title">Priorities</h1>
        <p class="page__subtitle">These values are loaded from the protected API and reused by the task editor.</p>
      </div>
    </header>

    <p v-if="loadError" class="notice notice--danger">{{ loadError }}</p>

    <div class="panel-grid">
      <section class="panel">
        <div class="panel__header">
          <h2 class="panel__title">{{ editingId ? 'Edit priority' : 'Create priority' }}</h2>
          <button v-if="editingId" type="button" class="button button--ghost button--small" @click="resetForm">
            Cancel
          </button>
        </div>

        <form class="form-stack" @submit.prevent="handleSubmit">
          <label class="field">
            <span class="field__label">Priority name</span>
            <input v-model.trim="form.priorityName" class="field__control" type="text" maxlength="128" required />
          </label>

          <label class="field">
            <span class="field__label">Sort order</span>
            <input v-model.number="form.prioritySort" class="field__control" type="number" min="0" required />
          </label>

          <label class="field">
            <span class="field__label">Tag</span>
            <input v-model.trim="form.tag" class="field__control" type="text" maxlength="255" />
          </label>

          <p v-if="saveError" class="notice notice--danger">{{ saveError }}</p>

          <button type="submit" class="button button--primary" :disabled="working">
            {{ working ? 'Saving...' : editingId ? 'Update priority' : 'Create priority' }}
          </button>
        </form>
      </section>

      <section class="panel">
        <div class="panel__header">
          <h2 class="panel__title">Existing priorities</h2>
          <button type="button" class="button button--ghost button--small" :disabled="prioritiesStore.loading" @click="loadPriorities">
            Reload
          </button>
        </div>

        <div v-if="prioritiesStore.items.length === 0" class="empty-state">
          No priorities yet. Add at least one before creating tasks.
        </div>

        <ul v-else class="plain-list plain-list--cards">
          <li v-for="item in prioritiesStore.items" :key="item.id" class="list-card list-card--stacked">
            <div class="list-card__head">
              <div>
                <strong>{{ item.priorityName }}</strong>
                <p>Sort: {{ item.prioritySort }}<span v-if="item.tag"> · Tag: {{ item.tag }}</span></p>
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
