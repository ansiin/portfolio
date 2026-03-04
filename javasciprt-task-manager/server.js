const http = require("http");
const fs = require("fs");
const path = require("path");

const DEFAULT_PORT = 4174;
const REQUESTED_PORT = process.env.PORT ? Number(process.env.PORT) : DEFAULT_PORT;
const ROOT = __dirname;

const MIME_TYPES = {
  ".html": "text/html; charset=utf-8",
  ".js": "application/javascript; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".json": "application/json; charset=utf-8",
  ".png": "image/png",
  ".jpg": "image/jpeg",
  ".jpeg": "image/jpeg",
  ".svg": "image/svg+xml",
  ".ico": "image/x-icon",
  ".txt": "text/plain; charset=utf-8",
};

function resolvePath(urlPath) {
  const cleanPath = decodeURIComponent((urlPath || "/").split("?")[0]);
  const relativePath = cleanPath === "/" ? "/index.html" : cleanPath;
  const absolutePath = path.normalize(path.join(ROOT, relativePath));
  if (!absolutePath.startsWith(ROOT)) {
    return null;
  }
  return absolutePath;
}

function createServer() {
  return http.createServer((req, res) => {
    const filePath = resolvePath(req.url);
    if (!filePath) {
      res.writeHead(403, { "Content-Type": "text/plain; charset=utf-8" });
      res.end("Forbidden");
      return;
    }

    fs.readFile(filePath, (err, data) => {
      if (err) {
        if (err.code === "ENOENT") {
          res.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
          res.end("Not Found");
          return;
        }
        res.writeHead(500, { "Content-Type": "text/plain; charset=utf-8" });
        res.end("Internal Server Error");
        return;
      }

      const ext = path.extname(filePath).toLowerCase();
      const contentType = MIME_TYPES[ext] || "application/octet-stream";
      res.writeHead(200, { "Content-Type": contentType });
      res.end(data);
    });
  });
}

function startServer(port, attemptsLeft) {
  const server = createServer();
  server.on("error", (error) => {
    const canTryNext = !process.env.PORT && error.code === "EADDRINUSE" && attemptsLeft > 0;
    if (!canTryNext) {
      throw error;
    }
    startServer(port + 1, attemptsLeft - 1);
  });

  server.listen(port, () => {
    console.log(`Task Manager running at http://localhost:${port}`);
  });
}

startServer(REQUESTED_PORT, 10);
