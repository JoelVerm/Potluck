FROM node:slim AS build

RUN corepack enable pnpm && corepack install -g pnpm

COPY . .
RUN rm -rf node_modules

RUN pnpm install
RUN pnpm build

FROM --platform=linux/arm64 lipanski/docker-static-website:latest

COPY --from=build /dist .
