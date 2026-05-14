FROM nginx:1.27-alpine

ARG API_BASE_URL=https://localhost:7192
WORKDIR /usr/share/nginx/html
COPY . .
RUN printf "window.INVESTING_TRACKER_CONFIG = { apiBaseUrl: \"%s\" };\n" "$API_BASE_URL" > /usr/share/nginx/html/config.js
