import type {Component, Signal} from 'solid-js'
import {createResource, For, Index} from 'solid-js'
import {Flex} from '~/components/ui/flex'
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from '~/components/ui/select'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import {apiCall, createInitUserListWS} from 'api'

const homeStatusOptions = ['At home', 'Away for a bit', 'Out of town'] as const

const Home: Component<{ username_signal: Signal<string> }> = props => {
    const [userName] = props.username_signal
    const [totalBalance] = createResource(() => apiCall('/users/current/balance', 'get'))
    const [eatingTotal, setEatingTotal] = createInitUserListWS('/users/current/eatingTotalPeople')
    const [homeStatus, setHomeStatus] = createInitUserListWS('/users/current/homeStatus')
    const [homeStatusList] = createResource(() =>
        apiCall('/users/homeStatus', 'get')
    )

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
                    eatingTotal().filter(v => v?.user === userName())[0]
                        ?.value ?? 0
                }
                setValue={setEatingTotal}
            />
            <FlexRow>
                <span>Right now I am</span>
                <Select
                    value={
                        homeStatus().filter(v => v?.user === userName())[0]
                            ?.value
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
