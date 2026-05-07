import express from "express";
import cors from "cors";
import helmet from "helmet";
import morgan from "morgan";
import { config } from "./config.js";
import { login, refresh, register, requireAuth } from "./auth.js";
import {
  createCategory,
  createPriority,
  createTask,
  deleteCategory,
  deletePriority,
  deleteTask,
  getCategory,
  getPriority,
  getTask,
  listCategories,
  listPriorities,
  listTasks,
  updateCategory,
  updatePriority,
  updateTask
} from "./todos.js";
import { problem } from "./problem.js";

export function createApp() {
  const app = express();

  app.use(helmet());
  app.use(cors({ origin: config.corsOrigin === "*" ? true : config.corsOrigin, credentials: true }));
  app.use(express.json({ limit: "128kb" }));
  app.use(morgan("combined"));

  app.get("/health", (_req, res) => {
    res.json({ status: "ok", service: "express-hw" });
  });

  for (const prefix of ["/api/v1", "/api/v1.0"]) {
    const router = express.Router();

    router.post("/Account/Login", login);
    router.post("/Account/Register", register);
    router.post("/Account/RefreshToken", refresh);

    router.use(requireAuth);

    router.get("/TodoCategories", listCategories);
    router.post("/TodoCategories", createCategory);
    router.get("/TodoCategories/:id", getCategory);
    router.put("/TodoCategories/:id", updateCategory);
    router.delete("/TodoCategories/:id", deleteCategory);

    router.get("/TodoPriorities", listPriorities);
    router.post("/TodoPriorities", createPriority);
    router.get("/TodoPriorities/:id", getPriority);
    router.put("/TodoPriorities/:id", updatePriority);
    router.delete("/TodoPriorities/:id", deletePriority);

    router.get("/TodoTasks", listTasks);
    router.post("/TodoTasks", createTask);
    router.get("/TodoTasks/:id", getTask);
    router.put("/TodoTasks/:id", updateTask);
    router.delete("/TodoTasks/:id", deleteTask);

    app.use(prefix, router);
  }

  app.use((req, res) => {
    problem(res, 404, "Not Found", `${req.method} ${req.originalUrl} was not found.`);
  });

  app.use((error, _req, res, _next) => {
    problem(res, 500, "Internal Server Error", error instanceof Error ? error.message : "Unhandled error.");
  });

  return app;
}
