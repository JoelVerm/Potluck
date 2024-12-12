# Potluck readme

Potluck is an app for students to keep track of who is cooking or eating. It is written in SolidJS with an asp.net backend.

## Running the app

The easiest way is using docker compose. Just run `docker compose up` in the main directory and the app should be live at `http://localhost`.

## Developing on the app

For development it is easier to run everything separately. All of those commands are in a single script which also starts caddy locally, so to run development mode just run `bash start.sh` in the main directory. The frontend should be live at `http://localhost`. The swagger UI should be at `http://localhost:4040/swagger` and the asyncapi JSON at `http://localhost:4040/asyncapi/asyncapi.json`.
