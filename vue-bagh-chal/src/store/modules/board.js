import axios from 'axios';
import bc from '../../baghchal/BaghChalGame';

export default {
  namespaced: true,
  state: {
    Board: [],
    //  [
    //   1, 0, 0, 0, 1,
    //   0, 0, 0, 0, 0,
    //   0, 0, 0, 0, 0,
    //   0, 0, 0, 0, 0,
    //   1, 0, 0, 0, 1,
    // ],
    turn: bc.PieceEnum.Goat,
    ply: 0,
    goatsLeftToPlace: 20,
    goatsCaptured: 0,
    GameWinner: -1,
    selectedPiece: null,
  },
  actions: {
    makeMove({ state, commit }, data) {
      // Pre checks on client.
      console.log(data);

      const indexOfItem = (data.r - 1) * 5 + (data.c - 1);
      const clicked = state.Board[indexOfItem];
      let callData = null;

      // If clicked an empty space and goats turn, try to place.
      if (clicked === bc.PieceEnum.Empty && state.turn === bc.PieceEnum.Goat) {
        callData = {
          xs: 0, ys: 0, xe: data.c, ye: data.r,
        };
      } else if (clicked === state.turn) {
        // Else if clicked an existing piece, mark it as selected.
        state.selectedPiece = data;
      } else if (clicked === bc.PieceEnum.Empty && state.selectedPiece) {
        // Else if attempting to move selected peice.
        callData = {
          xs: state.selectedPiece.c,
          ys: state.selectedPiece.r,
          xe: data.c,
          ye: data.r,
        };
      } else {
        // Not sure if this should be the defualt?
        console.log(
          `invalid move, t: ${state.turn} selectedPiece: ${state.selectedPiece}`,
        );
      }

      if (callData) {
        // Call backend to progress the game.
        axios.post('/api/game', callData)
          .then((result) => { console.log(result.data); if (result.data.result) { commit('updateGameState', result.data.newState); } })
          .catch(console.error);
      }
    },
    getState({ commit }) {
      axios.get('/api/game')
        .then(result => commit('setStartState', result.data))
        .catch(console.error);
    },
  },
  getters: {
  },
  mutations: {
    setStartState(state, data) {
      state.Board = data.board;
      state.turn = data.turn;
      state.ply = data.ply;
      state.goatsLeftToPlace = data.goatsLeftToPlace;
      state.goatsCaptured = data.goatsCaptured;
      state.GameWinner = data.gameWinner;
    },
    updateGameState(state, data) {
      state.Board = data.board;
      state.turn = data.turn;
      state.ply = data.ply;
      state.goatsLeftToPlace = data.goatsLeftToPlace;
      state.goatsCaptured = data.goatsCaptured;
      state.GameWinner = data.gameWinner;

      state.selectedPiece = null;
    },
  },
};
