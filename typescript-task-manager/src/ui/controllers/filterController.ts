import type { QueryInput } from "../../types/dto";
import type { TaskPriority, TaskStatus } from "../../types/entities";
import type { AppDom } from "../domBindings";

export type FilterController = {
  bind(): void;
};

export function createFilterController(
  dom: AppDom,
  onQuery: (query: QueryInput) => Promise<void>,
): FilterController {
  const getQuery = (): QueryInput => {
    const query: QueryInput = {
      sort: {
        field: dom.sortField.value as "title" | "createdAt" | "dueDate" | "priority" | "status",
        direction: dom.sortDirection.value as "asc" | "desc",
      },
    };

    const search = dom.searchInput.value.trim();
    if (search) {
      query.search = search;
    }

    const filter: QueryInput["filter"] = {};
    if (dom.statusFilter.value) {
      filter.status = dom.statusFilter.value as TaskStatus;
    }
    if (dom.priorityFilter.value) {
      filter.priority = dom.priorityFilter.value as TaskPriority;
    }
    if (dom.categoryFilter.value) {
      filter.categoryId = dom.categoryFilter.value;
    }
    if (dom.dueDateFilter.value) {
      filter.dueDate = dom.dueDateFilter.value;
    }
    const tag = dom.tagsFilter.value.trim();
    if (tag) {
      filter.tag = tag;
    }
    if (Object.keys(filter).length > 0) {
      query.filter = filter;
    }

    return query;
  };

  const bind = (): void => {
    dom.searchBtn.addEventListener("click", () => void onQuery(getQuery()));
    dom.applyFiltersBtn.addEventListener("click", () => void onQuery(getQuery()));
    dom.clearFiltersBtn.addEventListener("click", () => {
      dom.statusFilter.value = "";
      dom.priorityFilter.value = "";
      dom.categoryFilter.value = "";
      dom.dueDateFilter.value = "";
      dom.tagsFilter.value = "";
      dom.searchInput.value = "";
      dom.sortField.value = "createdAt";
      dom.sortDirection.value = "desc";
      void onQuery(getQuery());
    });
  };

  return { bind };
}
