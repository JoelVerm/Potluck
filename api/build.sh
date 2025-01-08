curl "http://localhost:4040/swagger/v1/swagger.json" -o api.json
pnpm npx openapi-typescript api.json --output ./src/api_def.ts
curl "http://localhost:4040/asyncapi/asyncapi.json" -o asyncapi.json
sed -i -e 's/"asyncapi": "2.4.0"/"openapi": "3.0.0"/g' asyncapi.json
pnpm npx openapi-typescript asyncapi.json --output ./src/ws_def.ts
rm api.json asyncapi.json
