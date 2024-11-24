import {createSignal} from 'solid-js'
import {paths} from './api_def'

export type Path = keyof paths
export type PathMethod<T extends Path> = keyof paths[T]

type RequestParams<
    P extends Path,
    M extends PathMethod<P>
> = paths[P][M] extends { parameters: { query: { [k: string]: any } } }
    ? paths[P][M]['parameters']['query']
    : undefined

type InnerRequestBody<T> = T extends { content: { 'application/json': any } } ? T['content']['application/json'] : never

type RequestBody<
    P extends Path,
    M extends PathMethod<P>
> = paths[P][M] extends { requestBody?: { content: { 'application/json': any } } }
    ? InnerRequestBody<paths[P][M]['requestBody']>
    : undefined

export type ResponseType<
    P extends Path,
    M extends PathMethod<P>
> = paths[P][M] extends {
        responses: { 200: { content: { 'application/json': any } } }
    }
    ? paths[P][M]['responses'][200]['content']['application/json']
    : undefined

export const apiCall = <P extends Path, M extends PathMethod<P>>(
    url: P,
    method: M,
    params?: RequestParams<P, M>,
    body?: RequestBody<P, M>
): Promise<ResponseType<P, M> | undefined> =>
    fetch(`/api${url}?` + new URLSearchParams(params), {
        method: method as string,
        headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json'
        },
        body: body == null ? undefined : JSON.stringify(body)
    })
        .then(res => (res.status == 200 ? res : undefined))
        .then(res => res?.json())
        .catch(err => (err instanceof SyntaxError ? '' : err)) // SyntaxError is thrown when the response is not JSON


export const createGetPostResource = <
    P extends Path,
    T extends ResponseType<P, 'get'> & RequestBody<P, 'post'>
>(
    url: P
): [() => T | undefined, (body: T) => void] => {
    const [value, setValue] = createSignal<T | undefined>(undefined)
    apiCall(url, 'get').then(setValue)
    const post = (body: T) => {
        if (body == undefined) return
        apiCall(url, 'post', undefined, body)
            .then(() => apiCall(url, 'get'))
            .then(setValue)
    }
    return [() => value(), post]
}
