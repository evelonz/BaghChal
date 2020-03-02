<template>
  <div class="white" :class="selected" v-on:click="clickPiece">
    <Piece v-bind:piece="GetIcon(this.piece)" />
  </div>
</template>

<script>
import Piece from './BaghChalPiece.vue';
import bc from './BaghChalGame';

export default {
  props: {
    piece: {
      type: Number,
      default: 0,
      required: true,
    },
    c: {
      type: Number,
      default: 0,
      required: true,
    },
    r: {
      type: Number,
      default: 0,
      required: true,
    },
    selected: {
      type: Boolean,
      default: false,
      required: false,
    },
  },
  components: {
    Piece,
  },
  methods: {
    clickPiece(event) {
      if (event) {
        console.log(
          `${this.c
          } ${
            this.r
          } ${
            this.piece
          } ${
            this.GetIcon(this.piece)}`,
        );
        // alert(event.target.tagName);
        // event.dataTransfer.setData("text", ev.target.id);
      }
      this.emitTileClicked();
    },
    GetIcon(intvalue) {
      return bc.GetIcon(intvalue);
    },
    emitTileClicked() {
      this.$emit('tileClicked', { r: this.r, c: this.c, piece: this.piece });
    },
  },
};
</script>

<style scoped>
.black {
  float: left;
  width: 80px;
  height: 80px;
  background-color: #999;
  font-size: 50px;
  text-align: center;
  display: table-cell;
  vertical-align: middle;
}
.white {
  float: left;
  width: 76px;
  height: 76px;
  background-color: #fff;
  font-size: 50px;
  text-align: center;
  display: table-cell;
  vertical-align: middle;
  border: 2px black solid;
}
.selected {
  border: 2px red solid !important;
}
</style>
