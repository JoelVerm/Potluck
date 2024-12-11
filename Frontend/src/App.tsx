import {Component, createEffect, createSignal, For, Show} from 'solid-js'
import {Tabs, TabsContent, TabsList, TabsTrigger} from '~/components/ui/tabs'

import Cooking from '~/Cooking'
import Home from '~/Home'
import Login from '~/Login'
import Settings from '~/Settings'
import Shopping from '~/Shopping'
import {client} from "api";

export type TabProps = {
    username: string
    houseName: string
}

const App: Component = () => {
    const [username, setUsername] = createSignal('')
    const [houseName, setHouseName] = createSignal('')
    createEffect(() => {
        const user = username()
        if (user.length <= 0)
            return
        client.GET(`/users/{name}/house`, {
            params: {
                path: {name: user}
            }
        }).then((houseName) => {
            setHouseName(houseName.data?.name ?? '')
        })
    })
    const createTabs = () => ({
        Home: <Home username={username()} houseName={houseName()}/>,
        Cooking: <Cooking username={username()} houseName={houseName()}/>,
        Shopping: <Shopping username={username()} houseName={houseName()}/>,
        Settings: <Settings username={username()} houseName={houseName()} setHouseName={setHouseName}/>
    })

    return (
        <Show
            when={username().length > 0}
            fallback={<Login username={username()} setUsername={setUsername} houseName={houseName()}/>}
        >
            {(() => {
                const tabs = createTabs()
                return (
                    <Tabs
                        defaultValue={Object.keys(tabs)[0]}
                        class="h-dvh grid"
                        style="grid-template-rows: 1fr auto"
                    >
                        <For each={Object.entries(tabs)}>
                            {([key, value]) => (
                                <TabsContent
                                    class="w-full max-w-md mx-auto p-2 my-0"
                                    value={key}
                                >
                                    {value}
                                </TabsContent>
                            )}
                        </For>
                        <TabsList class="rounded-none">
                            <div class="flex w-full max-w-md">
                                <For each={Object.keys(tabs)}>
                                    {key => (
                                        <TabsTrigger class="flex-1" value={key}>{key}</TabsTrigger>
                                    )}
                                </For>
                            </div>
                        </TabsList>
                    </Tabs>
                )
            })()
            }
        </Show>
    )
}

export default App
