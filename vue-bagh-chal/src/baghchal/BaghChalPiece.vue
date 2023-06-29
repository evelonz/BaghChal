<template>
  <span draggable="true" id="drag2" v-html="this.piece"> </span>
</template>

<script>
export default {
  props: {
    piece: {
      type: String,
      default: '🐐',
    },
  },
};
</script>

<style scoped></style>
