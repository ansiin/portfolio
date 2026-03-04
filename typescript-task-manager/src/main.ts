import { createTaskService } from "./bll/taskService";
import { createCategoryRepository } from "./dal/categoryRepository";
import { createStorageClient } from "./dal/storageClient";
import { createTaskRepository } from "./dal/taskRepository";
import { bindDom } from "./ui/domBindings";
import { createTaskController } from "./ui/controllers/taskController";

async function bootstrap(): Promise<void> {
  const storage = createStorageClient();
  const taskRepository = createTaskRepository(storage);
  const categoryRepository = createCategoryRepository(storage);
  const taskService = createTaskService(taskRepository, categoryRepository);
  const dom = bindDom();
  const controller = createTaskController(dom, taskService, categoryRepository);
  await controller.init();
}

void bootstrap();
