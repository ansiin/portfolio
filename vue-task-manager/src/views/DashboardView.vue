<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { RouterLink } from 'vue-router';

import { getErrorMessage } from '../lib/api';
import { formatDateTime } from '../lib/dates';
import { useCategoriesStore } from '../stores/categories';
import { usePrioritiesStore } from '../stores/priorities';
import { useTasksStore } from '../stores/tasks';

const categoriesStore = useCategoriesStore();
const prioritiesStore = usePrioritiesStore();
const tasksStore = useTasksStore();

const loading = ref(false);
const loadError = ref<string | null>(null);

const loadOverview = async () => {
  loading.value = true;
  loadError.value = null;

  try {
    await Promise.all([
      categoriesStore.loadItems(),
      prioritiesStore.loadItems(),
      tasksStore.loadItems(),
    ]);
  } catch (error) {
    loadError.value = getErrorMessage(error, 'Unable to load dashboard data');
  } finally {
    loading.value = false;
  }
};

onMounted(loadOverview);

const activeTasks = computed(() => tasksStore.items.filter((task) => !task.isCompleted && !task.isArchived));
const archivedTasks = computed(() => tasksStore.items.filter((task) => task.isArchived));
const completedTasks = computed(() => tasksStore.items.filter((task) => task.isCompleted && !task.isArchived));
const dueSoonTasks = computed(() =>
  [...activeTasks.value]
    .filter((task) => Boolean(task.dueDt))
    .sort((left, right) => new Date(left.dueDt || 0).getTime() - new Date(right.dueDt || 0).getTime())
    .slice(0, 5),
);
</script>

<template>
  <section class="page">
    <header class="page__header">
      <div>
        <p class="page__eyebrow">Overview</p>
        <h1 class="page__title">Project base status</h1>
        <p class="page__subtitle">
          This view gives you a quick read of the secured session and the current Todo data pulled from the API.
        </p>
      </div>

      <button type="button" class="button button--ghost" :disabled="loading" @click="loadOverview">
        {{ loading ? 'Refreshing...' : 'Refresh data' }}
      </button>
    </header>

    <p v-if="loadError" class="notice notice--danger">{{ loadError }}</p>

    <div class="stats-grid">
      <article class="stat-card">
        <span class="stat-card__label">Categories</span>
        <strong class="stat-card__value">{{ categoriesStore.items.length }}</strong>
      </article>

      <article class="stat-card">
        <span class="stat-card__label">Priorities</span>
        <strong class="stat-card__value">{{ prioritiesStore.items.length }}</strong>
      </article>

      <article class="stat-card">
        <span class="stat-card__label">Active tasks</span>
        <strong class="stat-card__value">{{ activeTasks.length }}</strong>
      </article>

      <article class="stat-card">
        <span class="stat-card__label">Completed</span>
        <strong class="stat-card__value">{{ completedTasks.length }}</strong>
      </article>
    </div>

    <div class="panel-grid">
      <section class="panel">
        <div class="panel__header">
          <div>
            <p class="page__eyebrow">Due next</p>
            <h2 class="panel__title">Upcoming deadlines</h2>
          </div>

          <RouterLink class="button button--ghost button--small" :to="{ name: 'tasks' }">Open tasks</RouterLink>
        </div>

        <div v-if="dueSoonTasks.length === 0" class="empty-state">
          No upcoming due dates yet. Add deadlines from the Tasks view.
        </div>

        <ul v-else class="plain-list plain-list--cards">
          <li v-for="task in dueSoonTasks" :key="task.id" class="list-card">
            <div>
              <strong>{{ task.taskName }}</strong>
              <p>{{ formatDateTime(task.dueDt) }}</p>
            </div>
            <span class="status-pill">Pending</span>
          </li>
        </ul>
      </section>

      <section class="panel">
        <div class="panel__header">
          <div>
            <p class="page__eyebrow">Structure</p>
            <h2 class="panel__title">Workspace health</h2>
          </div>
        </div>

        <ul class="plain-list plain-list--spaced">
          <li>
            <strong>{{ archivedTasks.length }}</strong> archived tasks kept for history
          </li>
          <li>
            <strong>{{ categoriesStore.items.length > 0 ? 'Ready' : 'Missing' }}</strong> category configuration
          </li>
          <li>
            <strong>{{ prioritiesStore.items.length > 0 ? 'Ready' : 'Missing' }}</strong> priority configuration
          </li>
          <li>
            <strong>{{ tasksStore.items.length > 0 ? 'Connected' : 'Empty' }}</strong> API data sync
          </li>
        </ul>
      </section>
    </div>
  </section>
</template>
