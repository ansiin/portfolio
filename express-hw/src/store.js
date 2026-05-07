import { mkdir, readFile, writeFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { config } from "./config.js";

const dataFile = resolve(config.dataFile);

let state = null;
let writeQueue = Promise.resolve();

function emptyState() {
  return {
    users: [],
    refreshTokens: [],
    todoCategories: [],
    todoPriorities: [],
    todoTasks: []
  };
}

export async function loadStore() {
  if (state) return state;

  try {
    state = JSON.parse(await readFile(dataFile, "utf8"));
  } catch {
    state = emptyState();
    await saveStore();
  }

  return state;
}

export async function saveStore() {
  await mkdir(dirname(dataFile), { recursive: true });
  writeQueue = writeQueue.then(() => writeFile(dataFile, JSON.stringify(state, null, 2)));
  await writeQueue;
}

export async function getStore() {
  return loadStore();
}
