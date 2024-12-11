import {components} from './ws_def'
import {createEffect, createRoot, createSignal} from 'solid-js'
import {createFinalURL, createQuerySerializer} from 'openapi-fetch'

type Schema = keyof components['schemas']
type Action = 'pub' | 'sub'
type Path = Schema extends `${infer Path}_${Action}` ? Path : undefined
type Type<P extends Path, A extends Action> = `${P}_${A}` extends Schema ? components['schemas'][`${P}_${A}`] : undefined

const createURL = (url: Path, params: { [key: string]: string }) => {
    const requiredPathParams = url.match(/{[^}]*}/g)?.map(param => param.slice(1, -1)) ?? []
    const pathParams: { [key: string]: string } = {}
    const queryParams: { [key: string]: string } = {}
    for (const item of Object.entries(params)) {
        const [key, value] = item
        if (value == "")
            return undefined
        if (requiredPathParams.includes(key)) {
            pathParams[key] = value
        } else {
            queryParams[key] = value
        }
    }
    if (Object.keys(pathParams).length != requiredPathParams.length) {
        return undefined
    }
    return createFinalURL(url, {
        baseUrl: "/api",
        params: {
            path: pathParams,
            query: queryParams
        },
        querySerializer: createQuerySerializer()
    })
}

export const readOnlyWS = <P extends Path>(
    path: P,
    params: () => { [key: string]: string },
    callback: (value: Type<P, 'sub'>) => void
) => {
    createRoot(() => {
        createEffect(() => {
            const url = createURL(path, params())
            if (url == undefined) return
            const ws = new WebSocket(url)
            ws.onmessage = event => {
                callback(JSON.parse(event.data))
            }
        })
    })
}

export const createWS = <P extends Path>(
    path: P,
    params: () => { [key: string]: string },
): [
    value: () => Type<P, 'sub'> | undefined,
    setValue: (data: Type<P, 'pub'>) => void
] => {
    const [value, setValue] = createSignal<Type<P, 'sub'>>()
    let send: ((data: Type<P, "pub">) => void) | undefined = undefined
    createEffect(() => {
        const url = createURL(path, params())
        if (url == undefined) {
            send = undefined
            return
        }
        const ws = new WebSocket(url)
        ws.onmessage = event => {
            setValue(JSON.parse(event.data))
        }
        send = (data: Type<P, 'pub'>) => {
            if (ws.readyState == ws.OPEN) ws.send(JSON.stringify(data))
        }
    })
    return [value, (data: Type<P, 'pub'>) => send?.(data)]
}
