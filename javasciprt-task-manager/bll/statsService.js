export function createStatsService() {
  function calculate(tasks) {
    const completed = tasks.filter((task) => task.status === "completed").length;
    return {
      total: tasks.length,
      completed,
      open: tasks.length - completed,
    };
  }

  return {
    calculate,
  };
}
