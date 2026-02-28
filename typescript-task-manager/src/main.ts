import { DependencyService } from "./bll/dependencyService";
import { QueryService } from "./bll/queryService";
import { RecurrenceService } from "./bll/recurrenceService";
import { StatsService } from "./bll/statsService";
import { TaskService } from "./bll/taskService";
import { CategoryRepository } from "./dal/categoryRepository";
import { StorageClient } from "./dal/storageClient";
import { TaskRepository } from "./dal/taskRepository";
import { bindDom } from "./ui/domBindings";
import { TaskController } from "./ui/controllers/taskController";

async function bootstrap(): Promise<void> {
  const storage = new StorageClient();
  const taskRepository = new TaskRepository(storage);
  const categoryRepository = new CategoryRepository(storage);
  const dependencyService = new DependencyService();
  const recurrenceService = new RecurrenceService();
  const statsService = new StatsService();
  const queryService = new QueryService();
  const taskService = new TaskService(
    taskRepository,
    categoryRepository,
    dependencyService,
    recurrenceService,
    statsService,
    queryService,
  );
  const dom = bindDom();
  const controller = new TaskController(dom, taskService, categoryRepository);
  await controller.init();
}

void bootstrap();
