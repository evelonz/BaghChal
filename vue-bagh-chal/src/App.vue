<template>
  <div id="app">
    <BaghChalBoard />
  </div>
</template>

<script>
import BaghChalBoard from './baghchal/BaghChalBoard.vue';

export default {
  name: 'app',
  components: {
    BaghChalBoard,
  },
};
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
