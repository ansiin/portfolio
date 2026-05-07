export function message(res, status, messages) {
  return res.status(status).json({ messages: Array.isArray(messages) ? messages : [messages] });
}

export function problem(res, status, title, detail = null, errors = null) {
  const body = {
    type: `https://tools.ietf.org/html/rfc9110#section-15.${status >= 500 ? "6" : "5"}.1`,
    title,
    status
  };

  if (detail) body.detail = detail;
  if (errors) body.errors = errors;

  return res.status(status).json(body);
}
