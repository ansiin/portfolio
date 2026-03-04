import { createQueryService } from "./bll/queryService.js";
import { createTaskService } from "./bll/taskService.js";
import { createStorageClient } from "./dal/storageClient.js";
import { createTaskRepository } from "./dal/taskRepository.js";
import { bindDom } from "./ui/dom.js";
import { createTaskController } from "./ui/controllers/taskController.js";

function bootstrap() {
  const storageClient = createStorageClient();
  const taskRepository = createTaskRepository(storageClient);

  const taskService = createTaskService({
    taskRepository,
    queryService: createQueryService(),
  });

  const dom = bindDom();
  const taskController = createTaskController(dom, taskService);
  taskController.init();
}

bootstrap();
