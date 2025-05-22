const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'http://' + process.env.VITE_API_HOST,
      changeOrigin: true,
    })
  ),
    app.use(
      '/swagger',
      createProxyMiddleware({
        target: 'http://' + process.env.VITE_API_HOST,
        changeOrigin: true,
      })
    );
};
