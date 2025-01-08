import {Component, createEffect, createSignal, For, Show} from 'solid-js'

import Cooking from '~/Cooking'
import Home from '~/Home'
import Login, {getUsernameCookie} from '~/Login'
import Settings from '~/Settings'
import Shopping from '~/Shopping'
import {client} from "api";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "~/components/ui/tabs";
import CreateHouse from "~/CreateHouse";

export type TabProps = {
    username: string
    houseName: string
}

const nonEmpty = (str: string) => str.length > 0 ? str : undefined

const App: Component = () => {
    const [username, setUsername] = createSignal(getUsernameCookie() ?? '')
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
    const [eatingTotal, setEatingTotal] = createSignal(0)
    const createTabs = (username: string, houseName: string) => ({
        Home: <Home username={username} houseName={houseName} setEatingTotal={setEatingTotal}/>,
        Cooking: <Cooking username={username} houseName={houseName} eatingTotal={eatingTotal()}/>,
        Shopping: <Shopping username={username} houseName={houseName}/>,
        Settings: <Settings username={username} houseName={houseName}/>
    })

    return (
        <Show
            when={nonEmpty(username())}
            fallback={<Login setUsername={setUsername}/>}
        >
            {(username) => <Show
                when={nonEmpty(houseName())}
                fallback={<CreateHouse setHouseName={setHouseName}/>}
            >
                {(houseName) => {
                    const tabs = createTabs(username(), houseName())
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
                                            <TabsTrigger class="flex-1" data-testid={`tab-${key}`}
                                                         value={key}>{key}</TabsTrigger>
                                        )}
                                    </For>
                                </div>
                            </TabsList>
                        </Tabs>
                    )
                }
                }
            </Show>}
        </Show>
    )
}

export default App
