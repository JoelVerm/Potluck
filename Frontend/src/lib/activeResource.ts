import type { Resource } from 'solid-js'
import { createResource, onCleanup } from 'solid-js'

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
    const [value, mutate] = pollingResource<T>(getUrl)
    const update = (updater: ResourceUpdate<T>) => {
        mutate((original?: T) => {
            const newValue = isFunction<ResourceUpdater<T>>(updater)
                ? updater(original)
                : updater
            fetch(postUrl, {
                method: 'POST',
                body: JSON.stringify(newValue)
            })
                .then(res => res.json())
                .then(newValue => mutate(newValue))
            return newValue
        })
    }
    return [value, update] as const
}

export function pollingResource<T>(getUrl: string) {
    const [value, { mutate, refetch }] = createResource<T>(() =>
        fetch(getUrl).then(res => res.json())
    )
    const timer = setInterval(() => {
        refetch()
    }, 2000)
    onCleanup(() => clearInterval(timer))
    return [value, mutate] as const
}
