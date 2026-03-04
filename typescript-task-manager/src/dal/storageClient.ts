import { dataAccessError } from "../shared/errors";
import { safeJsonParse } from "../shared/utils";

export type StorageClient = {
  read<T>(key: string, fallback: T): Promise<T>;
  write<T>(key: string, value: T): Promise<void>;
};

export function createStorageClient(): StorageClient {
  return {
    async read<T>(key: string, fallback: T): Promise<T> {
      try {
        const raw = localStorage.getItem(key);
        return safeJsonParse<T>(raw, fallback);
      } catch (error) {
        throw dataAccessError(
          `Failed to read key "${key}": ${error instanceof Error ? error.message : "unknown error"}`,
        );
      }
    },
    async write<T>(key: string, value: T): Promise<void> {
      try {
        localStorage.setItem(key, JSON.stringify(value));
      } catch (error) {
        throw dataAccessError(
          `Failed to write key "${key}": ${error instanceof Error ? error.message : "unknown error"}`,
        );
      }
    },
  };
}
