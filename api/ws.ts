import {components} from './ws_def'
import {createEffect, createSignal} from 'solid-js'
import {apiCall, Path as ApiPath} from "./api";

type Schema = keyof components['schemas']
type UserSchema = keyof {
    [P in Schema]: components['schemas'][P] extends {
        User?: string
    } ? P : never
}
type Action = 'pub' | 'sub'
type Path = Schema extends `${infer Path}_${Action}` ? Path : undefined
type UserPath = UserSchema extends `${infer Path}_${Action}` ? Path : undefined
type Type<P extends Path, A extends Action> = components['schemas'][`${P}_${A}`]

const _createWS = <P extends Path>(
    path: P,
    init: (() => Promise<Type<P, 'pub'>>) | undefined
): [
    value: () => Type<P, 'sub'> | undefined,
    setValue: (data: Type<P, 'pub'>) => void
] => {
    const ws = new WebSocket(`/api/ws${path}`)
    const [value, setValue] = createSignal<Type<P, 'sub'>>()
    ws.onmessage = event => {
        setValue(JSON.parse(event.data))
    }
    const send = (data: Type<P, 'pub'>) => {
        if (ws.readyState == ws.OPEN) ws.send(JSON.stringify(data))
    }
    if (init !== undefined)
        init().then(setValue)
    return [value, send]
}

export const createWS = <P extends Path>(
    path: P
) => _createWS(path, undefined)

export const createInitWS = <P extends Path & ApiPath>(
    path: P
) => _createWS<P>(path as P, () => (apiCall(path, 'get') as unknown as Promise<Type<P, 'pub'>>))

const _createUserListWS = <P extends Path & UserPath>(
    ws: [
        value: () => Type<P, 'sub'> | undefined,
        setValue: (data: Type<P, 'pub'>) => void
    ]
): [
    value: () => (Type<P, 'sub'> | undefined)[],
    setValue: (data: Type<P, 'pub'>) => void
] => {
    const [values, setValues] = createSignal<(Type<P, 'sub'> | undefined)[]>([])
    const [value, setValue] = ws
    createEffect(() => {
        const data = value()
        if (data) {
            // @ts-ignore
            setValues(values().filter(v => v?.User !== data.User))
            if (
                !((data as any).remove ||
                    (data as any).Remove ||
                    (data as any).delete ||
                    (data as any).Delete)
            )
                setValues(prev => [...prev, data])
        }
    })
    return [values, setValue]
}

export const createUserListWS = <P extends Path & UserPath>(
    path: P
) => _createUserListWS(createWS(path))

export const createInitUserListWS = <P extends Path & ApiPath & UserPath>(
    path: P
) => _createUserListWS(createInitWS(path))