import {Component, createEffect, createResource, createSignal, For, Index} from 'solid-js'
import {Flex} from '~/components/ui/flex'
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from '~/components/ui/select'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import {client, createWS, readOnlyWS} from 'api'
import {TabProps} from "~/App";

const homeStatusOptions = ['At home', 'Away for a bit', 'Out of town'] as const

const Home: Component<TabProps> = props => {
    const [totalBalance] = createResource(() => client.GET('/users/{name}/balance', {
        params: {
            path: {
                name: props.username
            }
        }
    }).then(res => res.data))
    const [eatingTotal, setEatingTotal] = createWS('/users/{name}/eatingTotalPeopleWS', () => ({
        name: props.username
    }))
    const [homeStatus, setHomeStatus] = createWS('/users/{name}/homeStatusWS', () => ({
        name: props.username
    }))
    const [homeStatusList, setHomeStatusList] = createSignal<{ [key: string]: string }>({})
    createEffect(async () => {
        if (props.houseName.length <= 0) return
        const res = await client.GET('/houses/{name}/users/homeStatus', {
            params: {
                path: {
                    name: props.houseName
                }
            }
        })
        if (res.data != undefined)
            setHomeStatusList(res.data)
    })
    createEffect(async () => {
        Object.entries(homeStatusList()).forEach(p => {
            readOnlyWS('/users/{name}/homeStatusWS', () => ({
                name: p[0]
            }), (status) => {
                if (status != undefined && status != p[1]) {
                    setHomeStatusList(prev => ({...prev, [p[0]]: status ?? ''}))
                }
            })
        })
    })
    
    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <h1>
                Your balance: 🍴{totalBalance()?.cookingPoints ?? 0} 🪙
                {totalBalance()?.euros ?? 0}
            </h1>
            <NumberRow
                text="Eating with"
                value={
                    eatingTotal() ?? 0
                }
                setValue={setEatingTotal}
            />
            <FlexRow>
                <span>Right now I am</span>
                <Select
                    value={
                        homeStatus()
                    }
                    onChange={v => setHomeStatus(v!)}
                    options={homeStatusOptions.slice()}
                    defaultValue={homeStatusOptions[0]}
                    disallowEmptySelection={true}
                    placeholder="Set your status"
                    itemComponent={props => (
                        <SelectItem item={props.item}>
                            {props.item.rawValue}
                        </SelectItem>
                    )}
                >
                    <SelectTrigger aria-label="Home status" class="w-36">
                        <SelectValue<string>>
                            {state => state.selectedOption()}
                        </SelectValue>
                    </SelectTrigger>
                    <SelectContent/>
                </Select>
            </FlexRow>
            <div>
                <Index each={homeStatusOptions}>
                    {option => (
                        <div class="my-2">
                            <h1>{option()}</h1>
                            <For
                                each={Object.entries(homeStatusList() ?? {})
                                    .filter(e => e[1] === option())
                                    .sort((a, b) => a[0].localeCompare(b[0]))}
                            >
                                {e => (
                                    <p class="text-muted-foreground">{e[0]}</p>
                                )}
                            </For>
                        </div>
                    )}
                </Index>
            </div>
        </Flex>
    )
}

export default Home
