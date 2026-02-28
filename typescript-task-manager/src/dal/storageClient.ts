import type { IStorageClient } from "../api/dal/IStorageClient";
import { DataAccessError } from "../shared/errors";
import { safeJsonParse } from "../shared/utils";

export class StorageClient implements IStorageClient {
  async read<T>(key: string, fallback: T): Promise<T> {
    try {
      const raw = localStorage.getItem(key);
      return safeJsonParse<T>(raw, fallback);
    } catch (error) {
      throw new DataAccessError(
        `Failed to read key "${key}": ${error instanceof Error ? error.message : "unknown error"}`,
      );
    }
  }

  async write<T>(key: string, value: T): Promise<void> {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      throw new DataAccessError(
        `Failed to write key "${key}": ${error instanceof Error ? error.message : "unknown error"}`,
      );
    }
  }
}
