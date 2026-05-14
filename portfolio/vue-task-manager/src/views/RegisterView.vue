<script setup lang="ts">
import { reactive, ref } from 'vue';
import { RouterLink, useRouter } from 'vue-router';

import { getErrorMessage } from '../lib/api';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();
const router = useRouter();

const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
});

const errorMessage = ref<string | null>(null);

const handleSubmit = async () => {
  errorMessage.value = null;

  if (form.password !== form.confirmPassword) {
    errorMessage.value = 'Passwords do not match.';
    return;
  }

  try {
    await authStore.register({
      firstName: form.firstName.trim(),
      lastName: form.lastName.trim(),
      email: form.email.trim(),
      password: form.password,
    });

    await router.push({ name: 'dashboard' });
  } catch (error) {
    errorMessage.value = getErrorMessage(error, 'Unable to create account');
  }
};
</script>

<template>
  <div class="auth-layout">
    <section class="auth-panel auth-panel--feature">
      <p class="auth-panel__eyebrow">Account bootstrap</p>
      <h1 class="auth-panel__title">Register once and reuse this foundation in your own projects.</h1>
      <p class="auth-panel__lead">
        The app is split into clear stores, typed API models and path-based routing so the base stays reusable after the course.
      </p>
    </section>

    <section class="auth-panel">
      <p class="auth-panel__eyebrow">Register</p>
      <h2 class="auth-panel__form-title">Create a new secure session</h2>

      <form class="form-stack" @submit.prevent="handleSubmit">
        <div class="form-grid">
          <label class="field">
            <span class="field__label">First name</span>
            <input v-model.trim="form.firstName" class="field__control" type="text" autocomplete="given-name" required />
          </label>

          <label class="field">
            <span class="field__label">Last name</span>
            <input v-model.trim="form.lastName" class="field__control" type="text" autocomplete="family-name" required />
          </label>
        </div>

        <label class="field">
          <span class="field__label">Email</span>
          <input v-model.trim="form.email" class="field__control" type="email" autocomplete="email" required />
        </label>

        <div class="form-grid">
          <label class="field">
            <span class="field__label">Password</span>
            <input
              v-model="form.password"
              class="field__control"
              type="password"
              autocomplete="new-password"
              minlength="6"
              required
            />
          </label>

          <label class="field">
            <span class="field__label">Confirm password</span>
            <input
              v-model="form.confirmPassword"
              class="field__control"
              type="password"
              autocomplete="new-password"
              minlength="6"
              required
            />
          </label>
        </div>

        <p v-if="errorMessage" class="notice notice--danger">{{ errorMessage }}</p>

        <button type="submit" class="button button--primary" :disabled="authStore.working">
          {{ authStore.working ? 'Creating account...' : 'Create account' }}
        </button>
      </form>

      <p class="auth-panel__footer">
        Already registered?
        <RouterLink :to="{ name: 'login' }">Go back to sign in.</RouterLink>
      </p>
    </section>
  </div>
</template>
