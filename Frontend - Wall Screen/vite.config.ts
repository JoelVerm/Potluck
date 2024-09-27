import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
// import devtools from 'solid-devtools/vite';
import path from "path"

export default defineConfig({
  plugins: [
    /* 
    Uncomment the following line to enable solid-devtools.
    For more info see https://github.com/thetarnav/solid-devtools/tree/main/packages/extension#readme
    */
    // devtools(),
    solidPlugin(),
  ],
  resolve: {
    alias: {
      "~": path.resolve(__dirname, "./src")
    }
  },
  server: {
    port: 3100,
  },
  build: {
    target: 'esnext',
  },
});