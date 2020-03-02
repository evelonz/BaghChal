export default {
  GetIcon(piece) {
    if (piece === this.PieceEnum.Tiger) return '🐅';
    if (piece === this.PieceEnum.Goat) return '🐐';
    return '';
  },
  PieceEnum: {
    Tiger: 0,
    Goat: 1,
    Any: 2,
    Empty: 3,
  },
};
