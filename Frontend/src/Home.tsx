import type { Component } from 'solid-js'

import {
    createResource,
    createSignal,
    createEffect,
    For,
    Index
} from 'solid-js'
import { Flex } from '~/components/ui/flex'
import {
    NumberField,
    NumberFieldDecrementTrigger,
    NumberFieldIncrementTrigger,
    NumberFieldInput
} from '~/components/ui/number-field'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from '~/components/ui/select'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'

const homeStatusOptions = ['At home', 'Away for a bit', 'Out of town'] as const

type HomeStatus = (typeof homeStatusOptions)[number]
interface HomeStatusList {
    [index: string]: HomeStatus
}

const Home: Component = () => {
    const [weatherLocation, setWeatherLocation] = createSignal('London')
    const [weather] = createResource(weatherLocation, location =>
        fetch(
            `https://localhost/api/weatherforecast?location=${location}`
        ).then(res => res.json())
    )

    const [eatingTotal, setEatingTotal] = createSignal(0)
    const [homeStatus, setHomeStatus] = createSignal('')
    const [homeStatusList, _] = createSignal<HomeStatusList>({
        Jente: 'At home',
        Anne: 'Away for a bit',
        Maarten: 'Out of town',
        Olaf: 'At home',
        Selina: 'Out of town',
        Misha: 'Away for a bit',
        Anneke: 'Away for a bit',
        Joël: 'At home'
    })

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <NumberRow
                text="Eating with"
                value={eatingTotal()}
                setValue={setEatingTotal}
            />
            <FlexRow>
                <span>Right now I am</span>
                <Select
                    value={homeStatus()}
                    onChange={setHomeStatus}
                    options={homeStatusOptions.slice()}
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
                                each={Object.entries(homeStatusList())
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
