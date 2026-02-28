const CATEGORIES_KEY = "categories";

export function createCategoryRepository(storageClient) {
  function getAll() {
    return storageClient.read(CATEGORIES_KEY, []);
  }

  function save(category) {
    const categories = getAll();
    const index = categories.findIndex((item) => item.id === category.id);
    if (index >= 0) {
      categories[index] = category;
    } else {
      categories.push(category);
    }
    storageClient.write(CATEGORIES_KEY, categories);
    return category;
  }

  return {
    getAll,
    save,
  };
}
