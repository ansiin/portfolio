FROM node:20-alpine AS ts-build
WORKDIR /app/typescript-task-manager

COPY typescript-task-manager/package*.json ./
RUN npm ci

COPY typescript-task-manager ./
RUN npm run build

FROM nginx:alpine

COPY deploy/nginx.conf /etc/nginx/nginx.conf
COPY deploy/index.html /usr/share/nginx/html/index.html

COPY javasciprt-task-manager/index.html /usr/share/nginx/html/javascript-task-manager/index.html
COPY javasciprt-task-manager/main.js /usr/share/nginx/html/javascript-task-manager/main.js
COPY javasciprt-task-manager/styles.css /usr/share/nginx/html/javascript-task-manager/styles.css
COPY javasciprt-task-manager/bll /usr/share/nginx/html/javascript-task-manager/bll
COPY javasciprt-task-manager/dal /usr/share/nginx/html/javascript-task-manager/dal
COPY javasciprt-task-manager/shared /usr/share/nginx/html/javascript-task-manager/shared
COPY javasciprt-task-manager/ui /usr/share/nginx/html/javascript-task-manager/ui

COPY --from=ts-build /app/typescript-task-manager/index.html /usr/share/nginx/html/typescript-task-manager/index.html
COPY --from=ts-build /app/typescript-task-manager/styles.css /usr/share/nginx/html/typescript-task-manager/styles.css
COPY --from=ts-build /app/typescript-task-manager/dist /usr/share/nginx/html/typescript-task-manager/dist
