<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink, RouterView, useRouter } from 'vue-router';

import { APP_NAME } from '../lib/config';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();
const router = useRouter();

const navigationItems = [
  { name: 'dashboard', label: 'Overview', to: { name: 'dashboard' } },
  { name: 'tasks', label: 'Tasks', to: { name: 'tasks' } },
  { name: 'categories', label: 'Categories', to: { name: 'categories' } },
  { name: 'priorities', label: 'Priorities', to: { name: 'priorities' } },
];

const displayName = computed(() => authStore.fullName || authStore.session?.email || 'Signed-in user');

const handleLogout = async () => {
  authStore.logout();
  await router.push({ name: 'login' });
};
</script>

<template>
  <div class="shell">
    <aside class="shell__sidebar">
      <div class="shell__brand">
        <p class="shell__eyebrow">TalTech ToDo API</p>
        <h1>{{ APP_NAME }}</h1>
        <p class="shell__description">
          JWT + refresh-token Vue 3 base with route guards, central API handling and modular Pinia stores.
        </p>
      </div>

      <nav class="shell__nav" aria-label="Main navigation">
        <RouterLink
          v-for="item in navigationItems"
          :key="item.name"
          :to="item.to"
          class="shell__nav-link"
        >
          {{ item.label }}
        </RouterLink>
      </nav>
    </aside>

    <section class="shell__content">
      <header class="shell__header">
        <div>
          <p class="shell__eyebrow">Authenticated session</p>
          <h2 class="shell__user">{{ displayName }}</h2>
          <p class="shell__session-meta">
            Token valid until <strong>{{ authStore.sessionExpiresLabel }}</strong>
          </p>
        </div>

        <button type="button" class="button button--ghost" @click="handleLogout">Log out</button>
      </header>

      <main class="shell__main">
        <RouterView />
      </main>
    </section>
  </div>
</template>
