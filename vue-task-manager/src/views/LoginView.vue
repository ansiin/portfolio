<script setup lang="ts">
import { reactive, ref } from 'vue';
import { RouterLink, useRoute, useRouter } from 'vue-router';

import { getErrorMessage } from '../lib/api';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();
const route = useRoute();
const router = useRouter();

const form = reactive({
  email: '',
  password: '',
});

const errorMessage = ref<string | null>(null);

const handleSubmit = async () => {
  errorMessage.value = null;

  try {
    await authStore.login(form);
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/app';
    await router.push(redirect);
  } catch (error) {
    errorMessage.value = getErrorMessage(error, 'Unable to sign in');
  }
};
</script>

<template>
  <div class="auth-layout">
    <section class="auth-panel auth-panel--feature">
      <p class="auth-panel__eyebrow">Vue 3 starter</p>
      <h1 class="auth-panel__title">Secure task management against the TalTech backend.</h1>
      <p class="auth-panel__lead">
        Access tokens are attached centrally, expired sessions are refreshed once, and route guards keep protected pages closed.
      </p>

      <div class="auth-callout">
        <h2>Security base included</h2>
        <ul class="plain-list">
          <li>JWT bearer auth on every protected request</li>
          <li>Refresh-token rotation on `401` or near-expiry startup</li>
          <li>Persistent session restore from local storage</li>
          <li>Router-level access control</li>
        </ul>
      </div>
    </section>

    <section class="auth-panel">
      <p class="auth-panel__eyebrow">Sign in</p>
      <h2 class="auth-panel__form-title">Continue to your workspace</h2>

      <form class="form-stack" @submit.prevent="handleSubmit">
        <label class="field">
          <span class="field__label">Email</span>
          <input v-model.trim="form.email" class="field__control" type="email" autocomplete="email" required />
        </label>

        <label class="field">
          <span class="field__label">Password</span>
          <input
            v-model="form.password"
            class="field__control"
            type="password"
            autocomplete="current-password"
            required
          />
        </label>

        <p v-if="errorMessage" class="notice notice--danger">{{ errorMessage }}</p>

        <button type="submit" class="button button--primary" :disabled="authStore.working">
          {{ authStore.working ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <p class="auth-panel__footer">
        No account yet?
        <RouterLink :to="{ name: 'register' }">Create one here.</RouterLink>
      </p>
    </section>
  </div>
</template>
