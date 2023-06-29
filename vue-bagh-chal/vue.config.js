module.exports = {
  devServer: {
    proxy: {
      '/api': {
        target: 'https://localhost:44328',
        changeOrigin: true,
      },
    },
  },
};
