FROM node:slim AS build

RUN corepack enable pnpm && corepack install -g pnpm

COPY ./api /api
COPY ./Frontend /frontend

WORKDIR /api
RUN pnpm i

WORKDIR /frontend
RUN pnpm i
RUN pnpm build

FROM --platform=linux/arm64 lipanski/docker-static-website:latest

COPY --from=build /frontend/dist .
