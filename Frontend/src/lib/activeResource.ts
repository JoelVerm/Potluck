import type { Resource } from 'solid-js'
import { createSignal, createResource, onCleanup } from 'solid-js'

const loggedInSignal = createSignal(true)
export const loggedIn = loggedInSignal[0]
const setLoggedIn = loggedInSignal[1]

type ResourceUpdater<T> = (original?: T) => T
type ResourceUpdate<T> = ResourceUpdater<T> | T
type ActiveResource<T> = readonly [
    Resource<T>,
    (updater: ResourceUpdate<T>) => void
]

function isFunction<T>(maybeFunc: T | unknown): maybeFunc is T {
    return typeof maybeFunc === 'function'
}

export function activeResource<T>(getUrl: string): ActiveResource<T>
export function activeResource<T>(
    getUrl: string,
    postUrl: string
): ActiveResource<T>
export function activeResource<T>(
    getUrl: string,
    postUrl?: string
): ActiveResource<T> {
    postUrl ??= getUrl
    const [value, mutate, registerUpdate, updateValid] =
        _pollingResource<T>(getUrl)
    const update = (updater: ResourceUpdate<T>) => {
        mutate((original?: T) => {
            const newValue = isFunction<ResourceUpdater<T>>(updater)
                ? updater(original)
                : updater
            const updateId = registerUpdate()
            fetch(postUrl, {
                method: 'POST',
                body: JSON.stringify(newValue),
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json'
                }
            })
                .then(res => res.json())
                .then(newValue => {
                    setLoggedIn(true)
                    if (updateValid(updateId)) mutate(newValue)
                })
                .catch(e => setLoggedIn(false))
            return newValue
        })
    }
    return [value, update] as const
}

export function pollingResource<T>(getUrl: string) {
    const [value, mutate] = _pollingResource<T>(getUrl)
    return [value, mutate] as const
}
function _pollingResource<T>(getUrl: string) {
    let updateCounter = 0
    const registerUpdate = () => ++updateCounter
    const updateValid = (id: number) => updateCounter === id
    const [value, { mutate, refetch }] = createResource<T>(async () => {
        const updateId = updateCounter
        const result = await fetch(getUrl)
            .then(res => res.json())
            .catch(e => {
                setLoggedIn(false)
                throw false
            })
        setLoggedIn(true)
        if (updateValid(updateId)) return result
        throw false
    })
    const timer = setInterval(() => {
        refetch()
    }, 2000)
    onCleanup(() => clearInterval(timer))
    return [value, mutate, registerUpdate, updateValid] as const
}
