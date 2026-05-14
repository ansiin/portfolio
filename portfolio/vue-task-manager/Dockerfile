FROM node:20-alpine AS build
WORKDIR /app

ARG VITE_API_BASE_URL=https://taltech.akaver.com/api/v1.0
ARG VITE_APP_BASE_PATH=/vue-task-manager/

ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_APP_BASE_PATH=$VITE_APP_BASE_PATH

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM nginx:alpine

COPY deploy/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
