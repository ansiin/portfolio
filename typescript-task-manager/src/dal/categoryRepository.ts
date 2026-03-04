import type { Category } from "../types/entities";
import { generateId } from "../shared/utils";
import type { StorageClient } from "./storageClient";

const CATEGORIES_KEY = "categories";

const DEFAULT_CATEGORIES: Category[] = [
  { id: generateId(), name: "Work", createdAt: new Date().toISOString() },
  { id: generateId(), name: "Personal", createdAt: new Date().toISOString() },
  { id: generateId(), name: "Study", createdAt: new Date().toISOString() },
];

export type CategoryRepository = {
  getAll(): Promise<Category[]>;
  getById(id: string): Promise<Category | null>;
};

export function createCategoryRepository(storage: StorageClient): CategoryRepository {
  const getAll = async (): Promise<Category[]> => {
    const categories = await storage.read<Category[]>(CATEGORIES_KEY, []);
    if (categories.length === 0) {
      await storage.write(CATEGORIES_KEY, DEFAULT_CATEGORIES);
      return DEFAULT_CATEGORIES;
    }
    return categories;
  };

  const getById = async (id: string): Promise<Category | null> => {
    const categories = await getAll();
    return categories.find((category) => category.id === id) ?? null;
  };

  return { getAll, getById };
}
