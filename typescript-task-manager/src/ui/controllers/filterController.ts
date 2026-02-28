import type { IFilterController } from "../../api/ui/IFilterController";
import type { QueryInput } from "../../types/dto";
import type { TaskPriority, TaskStatus } from "../../types/entities";
import type { AppDom } from "../domBindings";

export class FilterController implements IFilterController {
  constructor(
    private readonly dom: AppDom,
    private readonly onQuery: (query: QueryInput) => Promise<void>,
  ) {}

  bind(): void {
    this.dom.searchBtn.addEventListener("click", () => void this.onQuery(this.getQuery()));
    this.dom.applyFiltersBtn.addEventListener("click", () => void this.onQuery(this.getQuery()));
    this.dom.clearFiltersBtn.addEventListener("click", () => {
      this.dom.statusFilter.value = "";
      this.dom.priorityFilter.value = "";
      this.dom.categoryFilter.value = "";
      this.dom.dueDateFilter.value = "";
      this.dom.tagsFilter.value = "";
      this.dom.searchInput.value = "";
      this.dom.sortField.value = "createdAt";
      this.dom.sortDirection.value = "desc";
      void this.onQuery(this.getQuery());
    });
  }

  private getQuery(): QueryInput {
    const query: QueryInput = {
      sort: {
        field: this.dom.sortField.value as "title" | "createdAt" | "dueDate" | "priority" | "status",
        direction: this.dom.sortDirection.value as "asc" | "desc",
      },
    };

    const search = this.dom.searchInput.value.trim();
    if (search) {
      query.search = search;
    }

    const filter: QueryInput["filter"] = {};
    if (this.dom.statusFilter.value) {
      filter.status = this.dom.statusFilter.value as TaskStatus;
    }
    if (this.dom.priorityFilter.value) {
      filter.priority = this.dom.priorityFilter.value as TaskPriority;
    }
    if (this.dom.categoryFilter.value) {
      filter.categoryId = this.dom.categoryFilter.value;
    }
    if (this.dom.dueDateFilter.value) {
      filter.dueDate = this.dom.dueDateFilter.value;
    }
    const tag = this.dom.tagsFilter.value.trim();
    if (tag) {
      filter.tag = tag;
    }
    if (Object.keys(filter).length > 0) {
      query.filter = filter;
    }

    return query;
  }
}
