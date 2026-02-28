# Task Management System (Modular JS)

A pure JavaScript task manager refactored into DAL/BLL/UI/shared architecture.

## Features

- CRUD operations for tasks
- Task properties: `id`, `title`, `description`, `status`, `priority`, `dueDate`, `tags[]`
- `localStorage` persistence
- Search and filtering
- Input validation
- Clear layer separation for maintainability

## Project Structure

```txt
.
├── index.html
├── main.js
├── dal/
│   ├── storageClient.js
│   ├── taskRepository.js
│   └── categoryRepository.js
├── bll/
│   ├── taskService.js
│   ├── queryService.js
│   ├── dependencyService.js
│   ├── recurrenceService.js
│   └── statsService.js
├── ui/
│   ├── dom.js
│   ├── controllers/
│   │   ├── taskController.js
│   │   └── filterController.js
│   └── views/
│       ├── taskListView.js
│       ├── taskFormView.js
│       └── statsView.js
└── shared/
    ├── validators.js
    ├── utils.js
    └── errors.js
```

## Run

Use a local server (recommended for ES modules):

```bash
cd C:\dev\ansiin\javasciprt-task-manager
python -m http.server 4174
```

Open:

`http://localhost:4174`

Then use the UI to create, edit, filter, search, and delete tasks.
