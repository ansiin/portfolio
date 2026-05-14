function createNamedError(name: string, message: string): Error {
  const error = new Error(message);
  error.name = name;
  return error;
}

export function validationError(message: string): Error {
  return createNamedError("ValidationError", message);
}

export function notFoundError(message: string): Error {
  return createNamedError("NotFoundError", message);
}

export function dependencyError(message: string): Error {
  return createNamedError("DependencyError", message);
}

export function dataAccessError(message: string): Error {
  return createNamedError("DataAccessError", message);
}
