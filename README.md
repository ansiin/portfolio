# Task Management System

A simple, pure JavaScript task management system with web interface.

## Features

- **CRUD Operations**: Add, list, update, and delete tasks
- **Task Properties**: id, title, description, status, priority, dueDate, tags[]
- **Storage**: Tasks are stored in browser's localStorage
- **Filtering**: Filter tasks by status, priority, due date, or tags
- **Search**: Search tasks by title, description, tags, status, or priority
- **Validation**: Comprehensive input validation
- **Web Interface**: Responsive web interface for visual task management
- **Async Operations**: All operations are asynchronous with error handling

## Project Structure

```
├── index.html          # Web interface (runs in browser)
├── package.json        # Project dependencies and scripts
├── promts.md           # AI-assisted development prompts
└── README.md           # This file
```

## Usage

### Web Interface

1. Open `index.html` in any modern web browser
2. Use the interface to manage your tasks visually

## Task Properties

Each task has the following properties:

- **id**: Unique identifier (auto-generated)
- **title**: Task title (required, max 200 characters)
- **description**: Detailed description (optional, max 1000 characters)
- **status**: Task status (valid values: todo, in-progress, completed, cancelled)
- **priority**: Task priority (valid values: low, medium, high, urgent)
- **dueDate**: Due date (optional, format: YYYY-MM-DD)
- **tags**: Array of tags (optional)

## AI-Assisted Development

This project was developed using AI assistance. All prompts used during development are saved in `promts.md`.

## License

MIT
