import { createResource, onCleanup } from 'solid-js'

export function activeResource<T>(getUrl: string, postUrl: string) {
    const [value, mutate] = pollingResource<T>(getUrl)
    const update = (updater: (original?: T) => T) => {
        mutate((original?: T) => {
            const newValue = updater(original)
            fetch(postUrl, {
                method: 'POST',
                body: JSON.stringify(newValue)
            })
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
