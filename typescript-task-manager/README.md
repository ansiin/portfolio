# Task Manager (TypeScript, Strict)

Pure TypeScript task manager with DAL/BLL/UI architecture and browser `localStorage` persistence.

## Requirements Covered

- Full TypeScript conversion with strict mode
- Custom types for all entities
- Generic utility functions (3+)
- Recurring tasks
- Task dependencies
- Statistics
- Search and sorting
- Category +-< Task >-+ Priority relationships

## Architecture

```txt
src/
  types/
  dal/
  bll/
  ui/
  shared/
  main.ts
```

- `dal`: storage and repositories only (factory functions)
- `bll`: business rules and orchestration (pure functions + service factory)
- `ui`: rendering and event handling (controller factories)
- `types`: shared type aliases (`.d.ts`, no runtime output)

## Main Entities

- `Task`: `id, title, description, status, priority, dueDate, tags, categoryId, dependencyIds, recurrence`
- `Category`: `id, name`
- `TaskStatistics`: totals, completion, overdue, grouped counters

## Generic Utilities

Located in `src/shared/utils.ts`:

- `groupBy<T, K>()`
- `sortBy<T>()`
- `uniqueBy<T, K>()`
- `safeJsonParse<T>()` (extra)

## Run

1. Install dependencies:

```bash
npm install
```

2. Build:

```bash
npm run build
```

3. Start local server:

```bash
npm run serve
```

4. Open:

`http://localhost:4173`
