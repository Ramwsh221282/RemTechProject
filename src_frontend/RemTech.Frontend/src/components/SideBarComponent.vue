<script setup lang="ts">
import {Button} from "primevue";
</script>

<template>
  <div class="container">
    <Button class="menu-button">
      <div class="menu-button-content">
        <i class="pi pi-database" style="font-size: 1.5rem"></i>
        <span>Данные</span>
      </div>
    </Button>
    <Button class="menu-button">
      <div class="menu-button-content">
        <i class="pi pi-search" style="font-size: 1.5rem"></i>
        <span>Парсер</span>
      </div>
    </Button>
  </div>
</template>

<style scoped lang="scss">
@import "../assets/colors.scss";

.container {
  width: 15%;
  display: flex;
  flex-direction: column;
  font-weight: bold;
  font-size: 1rem;
  background-color: $amber-300;
  color: $amber-950;
  box-shadow: 0 0 10px 5px $amber-950;
  border-radius: 0.2rem;
}

:deep(.menu-button) {
  width: 100%;
  font-weight: bold;
  background-color: $amber-500;
  color: $amber-950;
  border-color: $amber-600;
  box-shadow: 0 0 1px 1px $amber-950;
  border-radius: 0 0 0 0;
}

.menu-button-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 0 1rem;
}

.menu-button-content i {
  flex-shrink: 0;
}

.menu-button-content span {
  flex-grow: 1;
  text-align: center;
}

</style>