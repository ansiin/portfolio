import type { ICategoryRepository } from "../api/dal/ICategoryRepository";
import type { IStorageClient } from "../api/dal/IStorageClient";
import type { Category } from "../types/entities";
import { generateId } from "../shared/utils";

const CATEGORIES_KEY = "categories";

const DEFAULT_CATEGORIES: Category[] = [
  { id: generateId(), name: "Work", createdAt: new Date().toISOString() },
  { id: generateId(), name: "Personal", createdAt: new Date().toISOString() },
  { id: generateId(), name: "Study", createdAt: new Date().toISOString() },
];

export class CategoryRepository implements ICategoryRepository {
  constructor(private readonly storage: IStorageClient) {}

  async getAll(): Promise<Category[]> {
    const categories = await this.storage.read<Category[]>(CATEGORIES_KEY, []);
    if (categories.length === 0) {
      await this.storage.write(CATEGORIES_KEY, DEFAULT_CATEGORIES);
      return DEFAULT_CATEGORIES;
    }
    return categories;
  }

  async getById(id: string): Promise<Category | null> {
    const categories = await this.getAll();
    return categories.find((category) => category.id === id) ?? null;
  }

  async save(category: Category): Promise<void> {
    const categories = await this.getAll();
    const index = categories.findIndex((item) => item.id === category.id);
    if (index >= 0) {
      categories[index] = category;
    } else {
      categories.push(category);
    }
    await this.storage.write(CATEGORIES_KEY, categories);
  }
}
