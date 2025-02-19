<script setup lang="ts">
import {Button} from "primevue";
import 'primeicons/primeicons.css'
</script>

<template>
  <div class="container">
    <div class="name-container">
      <strong>Application name place</strong>
    </div>
    <div class="middle-menus-container">
      <Button class="menu-button">Middle Menu button 1</Button>
      <Button class="menu-button">Middle Menu button 2</Button>
      <Button class="menu-button">Middle Menu button 3</Button>
      <Button class="menu-button">Middle Menu button 4</Button>
    </div>
    <div class="end-menus-container">
      <Button class="menu-button">End Menu button 1</Button>
      <Button class="menu-button">End Menu button 2</Button>
    </div>
  </div>
</template>

<style scoped lang="scss">
@import "../assets/colors.scss";

.container {
  width: 100%;
  display: inline-flex;
  flex-direction: row;
  padding: 1rem;
  font-weight: bold;
  font-size: 1rem;
  background-color: $amber-400;
  color: $amber-950;
  gap: 1rem;
  box-shadow: 0 0 10px 5px $amber-950;
  background-image: linear-gradient(135deg, $amber-400 0%, $amber-300 100%);
  border-radius: 0.2rem;
}

.middle-menus-container {
  margin-left: auto;
  display: inline-flex;
  gap: 1rem;
}

.end-menus-container {
  display: inline-flex;
  gap: 1rem;
  margin-left: auto;
}

.name-container {
  text-decoration: underline;
  align-content: center;
}

:deep(.menu-button) {
  font-weight: bold;
  background-color: $amber-500;
  color: $amber-950;
  border-color: $amber-600;
  box-shadow: 0 0 10px 1px $amber-950;
}
</style>