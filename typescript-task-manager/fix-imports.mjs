import fs from "node:fs";
import path from "node:path";

const distDir = path.resolve("dist");

function walk(dir) {
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      walk(fullPath);
      continue;
    }
    if (!entry.name.endsWith(".js")) {
      continue;
    }
    const content = fs.readFileSync(fullPath, "utf8");
    const updated = content.replace(
      /(from\s+["'])(\.[^"']+?)(["'])/g,
      (_, prefix, importPath, suffix) => {
        if (importPath.endsWith(".js")) {
          return `${prefix}${importPath}${suffix}`;
        }
        return `${prefix}${importPath}.js${suffix}`;
      },
    );
    if (updated !== content) {
      fs.writeFileSync(fullPath, updated, "utf8");
    }
  }
}

if (fs.existsSync(distDir)) {
  walk(distDir);
}
