import { createDependencyService } from "./bll/dependencyService.js";
import { createQueryService } from "./bll/queryService.js";
import { createRecurrenceService } from "./bll/recurrenceService.js";
import { createStatsService } from "./bll/statsService.js";
import { createTaskService } from "./bll/taskService.js";
import { createCategoryRepository } from "./dal/categoryRepository.js";
import { createStorageClient } from "./dal/storageClient.js";
import { createTaskRepository } from "./dal/taskRepository.js";
import { bindDom } from "./ui/dom.js";
import { createTaskController } from "./ui/controllers/taskController.js";

function bootstrap() {
  const storageClient = createStorageClient();
  const taskRepository = createTaskRepository(storageClient);
  createCategoryRepository(storageClient);

  const taskService = createTaskService({
    taskRepository,
    queryService: createQueryService(),
    statsService: createStatsService(),
    dependencyService: createDependencyService(),
    recurrenceService: createRecurrenceService(),
  });

  const dom = bindDom();
  const taskController = createTaskController(dom, taskService);
  taskController.init();
}

bootstrap();
