const TASKS_KEY = "tasks";

export function createTaskRepository(storageClient) {
  function getAll() {
    return storageClient.read(TASKS_KEY, []);
  }

  function getById(id) {
    return getAll().find((task) => task.id === id) || null;
  }

  function save(task) {
    const tasks = getAll();
    const index = tasks.findIndex((item) => item.id === task.id);
    if (index >= 0) {
      tasks[index] = task;
    } else {
      tasks.push(task);
    }
    storageClient.write(TASKS_KEY, tasks);
    return task;
  }

  function remove(id) {
    storageClient.write(
      TASKS_KEY,
      getAll().filter((task) => task.id !== id),
    );
  }

  function clear() {
    storageClient.write(TASKS_KEY, []);
  }

  return {
    getAll,
    getById,
    save,
    remove,
    clear,
  };
}
