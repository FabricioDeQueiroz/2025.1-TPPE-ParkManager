FROM node:20-alpine AS base
WORKDIR /app

COPY ParkManager-Frontend/package*.json ./

RUN npm install

EXPOSE 5173

CMD ["npm", "run", "dev:full"]