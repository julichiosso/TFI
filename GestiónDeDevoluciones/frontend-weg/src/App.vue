<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900 text-slate-900 dark:text-slate-100 font-sans">
    <RouterView />
  </div>
</template>

<style>
:root {
  --font-sans: 'Inter', system-ui, -apple-system, sans-serif;
}
</style>
