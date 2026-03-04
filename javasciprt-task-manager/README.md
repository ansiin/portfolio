# Task Management System (Modular JS)

A pure JavaScript task manager with DAL/BLL/UI/shared architecture.

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
|-- index.html
|-- main.js
|-- server.js
|-- styles.css
|-- dal/
|   |-- storageClient.js
|   `-- taskRepository.js
|-- bll/
|   |-- taskService.js
|   `-- queryService.js
|-- ui/
|   |-- dom.js
|   |-- controllers/
|   |   |-- filterController.js
|   |   `-- taskController.js
|   `-- views/
|       |-- taskFormView.js
|       `-- taskListView.js
`-- shared/
    |-- errors.js
    |-- utils.js
    `-- validators.js
```

## Run

Start with npm:

```bash
cd C:\dev\ansiin\javasciprt-task-manager
npm start
```

Open:

`http://localhost:4174`

Then use the UI to create, edit, filter, search, and delete tasks.
