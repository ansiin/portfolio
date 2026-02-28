export function createFilterController(dom, taskService, onTasksChanged) {
  function handleSearch() {
    const query = dom.searchInput.value.trim();
    if (!query) {
      onTasksChanged(taskService.list());
      return;
    }
    onTasksChanged(taskService.search(query));
  }

  function handleFilter() {
    const filters = {};
    if (dom.statusFilter.value) {
      filters.status = dom.statusFilter.value;
    }
    if (dom.priorityFilter.value) {
      filters.priority = dom.priorityFilter.value;
    }
    if (dom.dueDateFilter.value) {
      filters.dueDate = dom.dueDateFilter.value;
    }
    if (dom.tagsFilter.value.trim()) {
      filters.tags = dom.tagsFilter.value
        .split(",")
        .map((tag) => tag.trim())
        .filter(Boolean);
    }
    onTasksChanged(taskService.filter(filters));
  }

  function clearFilters() {
    dom.statusFilter.value = "";
    dom.priorityFilter.value = "";
    dom.dueDateFilter.value = "";
    dom.tagsFilter.value = "";
    dom.searchInput.value = "";
    onTasksChanged(taskService.list());
  }

  function bind() {
    dom.searchBtn.addEventListener("click", () => {
      try {
        handleSearch();
      } catch (error) {
        alert(error.message);
      }
    });
    dom.applyFiltersBtn.addEventListener("click", () => {
      try {
        handleFilter();
      } catch (error) {
        alert(error.message);
      }
    });
    dom.clearFiltersBtn.addEventListener("click", clearFilters);
  }

  return {
    bind,
  };
}
