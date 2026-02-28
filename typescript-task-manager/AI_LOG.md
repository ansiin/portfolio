# AI Usage Log

## What Worked

- Breaking the project into DAL/BLL/UI early kept files small and easy to reason about.
- Defining contracts in `src/api/**` made implementation boundaries clear.
- Generic utilities (`groupBy`, `sortBy`, `uniqueBy`) reduced repeated logic in services.
- Keeping recurrence and dependency checks as separate services made `TaskService` simpler.

## What Did Not Work

- Initial implementation started in the parent folder instead of the actual project folder. This was corrected by moving files into `javasciprt-task-manager`.
- A first draft in UI filtering used overly complex type casting and had to be simplified.

## What Was Adjusted Manually

- Simplified data flow so UI only calls service methods.
- Kept async boundaries in repositories/services even though localStorage is synchronous.
- Reduced coupling by passing only interfaces into services/controllers.

## Validation Approach

- Type checking with strict TypeScript configuration.
- Manual browser testing for:
  - CRUD
  - search/filter/sort
  - dependency completion constraints
  - recurring task generation on completion
  - statistics refresh after operations
