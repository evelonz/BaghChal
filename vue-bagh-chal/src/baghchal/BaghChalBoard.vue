<template>
  <div>
    <h1 v-if="this.GameWinner !== -1"
      style="text-align: left;">Winner {{ this.GetIcon(this.GameWinner) }}!</h1>
    <h2 style="text-align: left;">Turn: {{ this.GetIcon(this.turn) }}</h2>
    <p style="text-align: left;">
      <span>Ply: {{ this.ply }}</span> |
      <span>goatsLeftToPlace: {{ this.goatsLeftToPlace }}</span> |
      <span>goatsCaptured: {{ this.goatsCaptured }}</span> |
      <span>selectedPiece: {{ this.selectedPiece }}</span>
    </p>
    <div class="chessboard">
      <Tile
        v-for="(item, index) in this.Board"
        v-bind:key="index"
        v-bind:c="(index % 5) + 1"
        v-bind:r="Math.floor(index / 5) + 1"
        v-bind:piece="item"
        v-on:tileClicked="makeMove"
      />
    </div>
  </div>
</template>

<script>
import { mapActions, mapState } from 'vuex';

import Tile from './BaghChalTile.vue';
import bc from './BaghChalGame';

export default {
  created() {
    this.$store.dispatch('board/getState');
  },
  components: {
    Tile,
  },
  computed: {
    ...mapState('board',
      ['Board', 'turn', 'ply', 'goatsLeftToPlace', 'goatsCaptured', 'GameWinner', 'selectedPiece']),
  },
  methods: {
    ...mapActions('board',
      ['makeMove']),
    GetIcon(intvalue) {
      return bc.GetIcon(intvalue);
    },
    IsSelectedPiece(index) {
      const selected = this.$store.state.board.selectedTiger;
      if (selected) {
        const selectedIndex = selected.r * 5 + selected.c;
        return selectedIndex === index;
      }
      return false;
    },
  },
};
</script>

<style scoped>
.chessboard {
  width: 400px;
  height: 400px;
  margin: 20px;
  border: 15px solid #333;
}
</style>
