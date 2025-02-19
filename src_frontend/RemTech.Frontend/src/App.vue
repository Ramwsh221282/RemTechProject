<script setup lang="ts">
import {RouterView} from 'vue-router'
import HeaderComponent from "@/components/HeaderComponent.vue";
import SideBarComponent from "@/components/SideBarComponent.vue";
</script>

<template>
  <div class="app-layout">
    <header-component></header-component>
    <div class="app-content">
      <SideBarComponent></SideBarComponent>
      <RouterView/>
    </div>
  </div>
</template>

<style scoped lang="scss">
.app-layout {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.app-content {
  display: flex;
  flex-direction: row;
  gap: 0.5rem;
  flex-grow: 1;
}
</style>
