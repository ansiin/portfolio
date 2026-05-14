import { DataAccessError } from "../shared/errors.js";

export function createStorageClient(storage = window.localStorage) {
  return {
    read(key, fallback = []) {
      try {
        const raw = storage.getItem(key);
        return raw ? JSON.parse(raw) : fallback;
      } catch (error) {
        throw new DataAccessError(`Failed to read "${key}": ${error.message}`);
      }
    },
    write(key, value) {
      try {
        storage.setItem(key, JSON.stringify(value));
      } catch (error) {
        throw new DataAccessError(`Failed to write "${key}": ${error.message}`);
      }
    },
  };
}
