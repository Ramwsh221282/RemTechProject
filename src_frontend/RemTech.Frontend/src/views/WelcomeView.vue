<script setup lang="ts">

</script>

<template>
  <div class="container">
    <p class="test">Welcome</p>
  </div>
</template>

<style scoped lang="scss">
@import "../assets/colors.scss";

.container {
  width: 100%;
  height: 100%;
  margin: auto;
  display: flex;
  flex-direction: column;
  align-self: center;
  justify-content: center;
  align-items: center;
  background-color: $amber-100;
  border-radius: 0.2rem;
}

.test {
  font-weight: bold;
  font-size: 1.5rem;
  color: black;
}
</style>