export function createDependencyService() {
  function canComplete() {
    return true;
  }

  return {
    canComplete,
  };
}
