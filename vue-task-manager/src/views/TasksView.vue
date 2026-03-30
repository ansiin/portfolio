<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';

import { getErrorMessage } from '../lib/api';
import { formatDate, formatDateTime, fromLocalDateTimeInput, toLocalDateTimeInput } from '../lib/dates';
import { useCategoriesStore } from '../stores/categories';
import { usePrioritiesStore } from '../stores/priorities';
import { useTasksStore } from '../stores/tasks';
import type { TodoTask } from '../types/todo';

const categoriesStore = useCategoriesStore();
const prioritiesStore = usePrioritiesStore();
const tasksStore = useTasksStore();

const filter = ref<'all' | 'active' | 'completed' | 'archived'>('active');
const saveError = ref<string | null>(null);
const loadError = ref<string | null>(null);
const working = ref(false);
const editingId = ref<string | null>(null);

const form = reactive({
  id: '',
  taskName: '',
  taskSort: 1,
  dueDt: '',
  isCompleted: false,
  isArchived: false,
  todoCategoryId: '',
  todoPriorityId: '',
  createdDt: '',
  syncDt: '',
});

const categoryMap = computed(() =>
  new Map(categoriesStore.items.map((item) => [item.id, item.categoryName])),
);

const priorityMap = computed(() =>
  new Map(prioritiesStore.items.map((item) => [item.id, item.priorityName])),
);

const filteredTasks = computed(() =>
  tasksStore.items.filter((task) => {
    switch (filter.value) {
      case 'active':
        return !task.isCompleted && !task.isArchived;
      case 'completed':
        return task.isCompleted && !task.isArchived;
      case 'archived':
        return task.isArchived;
      default:
        return true;
    }
  }),
);

const canEditTasks = computed(() => categoriesStore.items.length > 0 && prioritiesStore.items.length > 0);

const resetForm = () => {
  editingId.value = null;
  form.id = '';
  form.taskName = '';
  form.taskSort = tasksStore.items.length + 1 || 1;
  form.dueDt = '';
  form.isCompleted = false;
  form.isArchived = false;
  form.todoCategoryId = categoriesStore.items[0]?.id || '';
  form.todoPriorityId = prioritiesStore.items[0]?.id || '';
  form.createdDt = new Date().toISOString();
  form.syncDt = new Date().toISOString();
  saveError.value = null;
};

const loadData = async () => {
  loadError.value = null;

  try {
    await Promise.all([
      categoriesStore.loadItems(),
      prioritiesStore.loadItems(),
      tasksStore.loadItems(),
    ]);
  } catch (error) {
    loadError.value = getErrorMessage(error, 'Unable to load tasks');
  } finally {
    if (!editingId.value) {
      resetForm();
    }
  }
};

onMounted(loadData);

const startEditing = (task: TodoTask) => {
  editingId.value = task.id;
  form.id = task.id;
  form.taskName = task.taskName;
  form.taskSort = task.taskSort;
  form.dueDt = toLocalDateTimeInput(task.dueDt);
  form.isCompleted = task.isCompleted;
  form.isArchived = task.isArchived;
  form.todoCategoryId = task.todoCategoryId;
  form.todoPriorityId = task.todoPriorityId;
  form.createdDt = task.createdDt;
  form.syncDt = task.syncDt;
  saveError.value = null;
};

const buildPayload = () => ({
  id: editingId.value || crypto.randomUUID(),
  taskName: form.taskName.trim(),
  taskSort: Number(form.taskSort),
  dueDt: fromLocalDateTimeInput(form.dueDt),
  isCompleted: form.isCompleted,
  isArchived: form.isArchived,
  todoCategoryId: form.todoCategoryId,
  todoPriorityId: form.todoPriorityId,
  createdDt: form.createdDt || new Date().toISOString(),
  syncDt: new Date().toISOString(),
});

const handleSubmit = async () => {
  working.value = true;
  saveError.value = null;

  try {
    const payload = buildPayload();

    if (editingId.value) {
      await tasksStore.updateItem(payload);
    } else {
      await tasksStore.createItem(payload);
    }

    resetForm();
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to save task');
  } finally {
    working.value = false;
  }
};

const toggleCompletion = async (task: TodoTask) => {
  try {
    await tasksStore.updateItem({
      ...task,
      isCompleted: !task.isCompleted,
      syncDt: new Date().toISOString(),
    });
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to update task');
  }
};

const toggleArchived = async (task: TodoTask) => {
  try {
    await tasksStore.updateItem({
      ...task,
      isArchived: !task.isArchived,
      syncDt: new Date().toISOString(),
    });
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to update task');
  }
};

const handleDelete = async (task: TodoTask) => {
  const confirmed = window.confirm(`Delete task "${task.taskName}"?`);
  if (!confirmed) {
    return;
  }

  try {
    await tasksStore.deleteItem(task.id);
    if (editingId.value === task.id) {
      resetForm();
    }
  } catch (error) {
    saveError.value = getErrorMessage(error, 'Unable to delete task');
  }
};
</script>

<template>
  <section class="page">
    <header class="page__header">
      <div>
        <p class="page__eyebrow">Protected CRUD</p>
        <h1 class="page__title">Tasks</h1>
        <p class="page__subtitle">
          Tasks depend on category and priority IDs, so this view only becomes usable after both lookup tables exist.
        </p>
      </div>

      <div class="action-row">
        <button type="button" class="button button--ghost" :disabled="tasksStore.loading" @click="loadData">Reload</button>
      </div>
    </header>

    <p v-if="loadError" class="notice notice--danger">{{ loadError }}</p>
    <p v-if="saveError" class="notice notice--danger">{{ saveError }}</p>

    <div v-if="!canEditTasks" class="notice notice--warning">
      Create at least one category and one priority before adding tasks.
    </div>

    <div class="panel-grid">
      <section class="panel">
        <div class="panel__header">
          <h2 class="panel__title">{{ editingId ? 'Edit task' : 'Create task' }}</h2>
          <button v-if="editingId" type="button" class="button button--ghost button--small" @click="resetForm">
            Cancel
          </button>
        </div>

        <form class="form-stack" @submit.prevent="handleSubmit">
          <label class="field">
            <span class="field__label">Task name</span>
            <input v-model.trim="form.taskName" class="field__control" type="text" maxlength="128" required :disabled="!canEditTasks" />
          </label>

          <div class="form-grid">
            <label class="field">
              <span class="field__label">Sort order</span>
              <input v-model.number="form.taskSort" class="field__control" type="number" min="0" required :disabled="!canEditTasks" />
            </label>

            <label class="field">
              <span class="field__label">Due date</span>
              <input v-model="form.dueDt" class="field__control" type="datetime-local" :disabled="!canEditTasks" />
            </label>
          </div>

          <div class="form-grid">
            <label class="field">
              <span class="field__label">Category</span>
              <select v-model="form.todoCategoryId" class="field__control" required :disabled="!canEditTasks">
                <option disabled value="">Select a category</option>
                <option v-for="item in categoriesStore.items" :key="item.id" :value="item.id">
                  {{ item.categoryName }}
                </option>
              </select>
            </label>

            <label class="field">
              <span class="field__label">Priority</span>
              <select v-model="form.todoPriorityId" class="field__control" required :disabled="!canEditTasks">
                <option disabled value="">Select a priority</option>
                <option v-for="item in prioritiesStore.items" :key="item.id" :value="item.id">
                  {{ item.priorityName }}
                </option>
              </select>
            </label>
          </div>

          <div class="checkbox-row">
            <label class="checkbox">
              <input v-model="form.isCompleted" type="checkbox" :disabled="!canEditTasks" />
              <span>Completed</span>
            </label>

            <label class="checkbox">
              <input v-model="form.isArchived" type="checkbox" :disabled="!canEditTasks" />
              <span>Archived</span>
            </label>
          </div>

          <button type="submit" class="button button--primary" :disabled="working || !canEditTasks">
            {{ working ? 'Saving...' : editingId ? 'Update task' : 'Create task' }}
          </button>
        </form>
      </section>

      <section class="panel">
        <div class="panel__header">
          <div>
            <p class="page__eyebrow">Loaded from API</p>
            <h2 class="panel__title">Task list</h2>
          </div>

          <div class="segmented-control" role="tablist" aria-label="Task filters">
            <button
              v-for="option in ['active', 'completed', 'archived', 'all']"
              :key="option"
              type="button"
              class="segmented-control__button"
              :class="{ 'segmented-control__button--active': filter === option }"
              @click="filter = option as 'all' | 'active' | 'completed' | 'archived'"
            >
              {{ option }}
            </button>
          </div>
        </div>

        <div v-if="filteredTasks.length === 0" class="empty-state">
          No tasks in this filter yet.
        </div>

        <ul v-else class="plain-list plain-list--cards">
          <li v-for="task in filteredTasks" :key="task.id" class="list-card list-card--stacked">
            <div class="list-card__head">
              <div>
                <strong>{{ task.taskName }}</strong>
                <p>
                  {{ categoryMap.get(task.todoCategoryId) || 'Unknown category' }} ·
                  {{ priorityMap.get(task.todoPriorityId) || 'Unknown priority' }}
                </p>
              </div>

              <div class="task-statuses">
                <span class="status-pill" :class="{ 'status-pill--success': task.isCompleted }">
                  {{ task.isCompleted ? 'Completed' : 'Pending' }}
                </span>
                <span v-if="task.isArchived" class="status-pill status-pill--muted">Archived</span>
              </div>
            </div>

            <div class="task-meta">
              <span>Due: {{ task.dueDt ? formatDateTime(task.dueDt) : 'No deadline' }}</span>
              <span>Created: {{ formatDate(task.createdDt) }}</span>
              <span>Sort: {{ task.taskSort }}</span>
            </div>

            <div class="action-row">
              <button type="button" class="button button--ghost button--small" @click="startEditing(task)">Edit</button>
              <button type="button" class="button button--ghost button--small" @click="toggleCompletion(task)">
                {{ task.isCompleted ? 'Mark active' : 'Mark done' }}
              </button>
              <button type="button" class="button button--ghost button--small" @click="toggleArchived(task)">
                {{ task.isArchived ? 'Unarchive' : 'Archive' }}
              </button>
              <button type="button" class="button button--danger button--small" @click="handleDelete(task)">Delete</button>
            </div>
          </li>
        </ul>
      </section>
    </div>
  </section>
</template>
