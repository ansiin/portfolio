# AI Usage Log

## What Worked

- Splitting the app into `dal`, `bll`, `ui`, and `types` kept responsibilities clear during migration.
- Enabling strict TypeScript options early (`strict`, `noImplicitAny`, `exactOptionalPropertyTypes`) exposed weak spots quickly.
- Generic helpers in `src/shared/utils.ts` (`groupBy`, `sortBy`, `uniqueBy`) reduced repeated logic in services.
- Keeping recurrence, dependency checks, query filtering, and statistics as separate modules made behavior easier to test manually.
- Factory-style composition in `src/main.ts` made dependency wiring explicit and simpler than deep class/interface layers.

## What Did Not Work

- Initial design used extra interface/class layers that added boilerplate without practical benefit for a single-app implementation.
- A refactor to simplify update mapping first broke under `exactOptionalPropertyTypes` and required conditional object spreading.
- Some filesystem operations (rename/move in PowerShell) failed due to environment restrictions, so changes were applied via direct patches instead.

## What Was Adjusted Manually

- Removed unused/overly abstract API contract layer (`src/api`) and migrated runtime logic to function/factory style.
- Simplified controller/repository/service construction to reduce indirection while preserving behavior.
- Rebuilt `dist` after cleanup to remove stale generated artifacts.
- Updated README architecture notes to reflect the final structure.

## Validation Approach

- Type checks: `tsc --noEmit` and `tsc --noEmit --noUnusedLocals --noUnusedParameters`
- Build check: `npm run build`
- Manual behavior checks in browser:
  - task create/update/delete/complete
  - dependency validation and recurrence generation
  - search/filter/sort
  - statistics updates after operations
