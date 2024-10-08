import type { Component } from 'solid-js'

import { For, Show } from 'solid-js'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'

import Cooking from '~/Cooking'
import Home from '~/Home'
import Login from '~/Login'
import Settings from '~/Settings'
import Shopping from '~/Shopping'

import { loggedIn } from '~/lib/activeResource'

const App: Component = () => {
    const tabs = {
        Home: <Home />,
        Cooking: <Cooking />,
        Shopping: <Shopping />,
        Settings: <Settings />
    }
    const tabsStyle = `grid-template-columns: repeat(${
        Object.keys(tabs).length
    }, minmax(0, 1fr));`

    return (
        <Show when={loggedIn()} fallback={<Login />}>
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
                    <div class="grid w-full max-w-md" style={tabsStyle}>
                        <For each={Object.keys(tabs)}>
                            {key => (
                                <TabsTrigger value={key}>{key}</TabsTrigger>
                            )}
                        </For>
                    </div>
                </TabsList>
            </Tabs>
        </Show>
    )
}

export default App
