FROM node:20-alpine

WORKDIR /app
ENV NODE_ENV=production
ENV PORT=4000
ENV DATA_FILE=/app/data/db.json

COPY package*.json ./
RUN npm ci --omit=dev

COPY src ./src
RUN mkdir -p /app/data

EXPOSE 4000
CMD ["node", "src/index.js"]
