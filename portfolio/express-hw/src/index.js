import { createApp } from "./app.js";
import { config } from "./config.js";
import { loadStore } from "./store.js";

await loadStore();

createApp().listen(config.port, "0.0.0.0", () => {
  console.log(`express-hw listening on ${config.port}`);
});
