import createClient from "openapi-fetch";
import {paths} from "./api_def";

export const client = createClient<paths>({
    baseUrl: "/api/",
});
