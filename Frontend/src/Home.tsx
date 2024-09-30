import type { Component } from 'solid-js'

import { For, Index } from 'solid-js'
import { Flex } from '~/components/ui/flex'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from '~/components/ui/select'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import { activeResource, pollingResource } from '~/lib/activeResource'

const homeStatusOptions = ['At home', 'Away for a bit', 'Out of town'] as const

type HomeStatus = (typeof homeStatusOptions)[number]
interface HomeStatusList {
    [index: string]: HomeStatus
}

const Home: Component = () => {
    const [totalBalance] =
        pollingResource<[number, number]>('/api/totalBalance')
    const [eatingTotal, setEatingTotal] =
        activeResource<number>('/api/eatingTotal')
    const [homeStatus, setHomeStatus] =
        activeResource<HomeStatus>('/api/homeStatus')
    const [homeStatusList] = pollingResource<HomeStatusList>(
        '/api/homeStatusList'
    )

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <h1>
                Your balance: 🍴{totalBalance()?.[0] ?? 0} 🪙
                {totalBalance()?.[1] ?? 0}
            </h1>
            <NumberRow
                text="Eating with"
                value={eatingTotal() ?? 0}
                setValue={setEatingTotal}
            />
            <FlexRow>
                <span>Right now I am</span>
                <Select
                    value={homeStatus()}
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
                    <SelectContent />
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
