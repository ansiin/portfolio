const PRIORITY_ORDER = ["low", "medium", "high", "urgent"];

export function createQueryService() {
  function filter(tasks, filterCriteria = {}) {
    return tasks.filter((task) => {
      return Object.entries(filterCriteria).every(([key, value]) => {
        if (key === "tags") {
          return value.every((tag) => task.tags.includes(tag));
        }
        if (key === "dueDate") {
          if (!task.dueDate) {
            return false;
          }
          return new Date(task.dueDate).toDateString() === new Date(value).toDateString();
        }
        if (key === "minPriority") {
          return PRIORITY_ORDER.indexOf(task.priority) >= PRIORITY_ORDER.indexOf(value);
        }
        return task[key] === value;
      });
    });
  }

  function search(tasks, query) {
    const term = String(query || "").toLowerCase();
    if (!term) {
      return tasks;
    }
    return tasks.filter((task) => {
      return (
        task.title.toLowerCase().includes(term) ||
        task.description.toLowerCase().includes(term) ||
        task.tags.some((tag) => tag.toLowerCase().includes(term)) ||
        task.status.toLowerCase().includes(term) ||
        task.priority.toLowerCase().includes(term)
      );
    });
  }

  return {
    filter,
    search,
  };
}
