import type { Category } from "../../types/entities";

export interface ICategoryRepository {
  getAll(): Promise<Category[]>;
  getById(id: string): Promise<Category | null>;
  save(category: Category): Promise<void>;
}
