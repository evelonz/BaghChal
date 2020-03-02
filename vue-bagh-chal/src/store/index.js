import Vue from 'vue';
import Vuex from 'vuex';

import boardModule from './modules/board';

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    board: boardModule,
  },
});
