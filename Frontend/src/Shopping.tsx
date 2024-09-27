import type { Component } from 'solid-js'

import {
    createResource,
    createSignal,
    createEffect,
    For,
    Index,
    createMemo
} from 'solid-js'

import { Flex } from '~/components/ui/flex'
import {
    Switch,
    SwitchControl,
    SwitchLabel,
    SwitchThumb
} from '~/components/ui/switch'
import {
    NumberField,
    NumberFieldDecrementTrigger,
    NumberFieldIncrementTrigger,
    NumberFieldInput
} from '~/components/ui/number-field'
import {
    TextField,
    TextFieldTextArea,
    TextFieldInput,
    TextFieldLabel
} from '~/components/ui/text-field'
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle
} from '~/components/ui/card'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from '~/components/ui/select'
import { Separator } from '~/components/ui/separator'
import { Button } from '~/components/ui/button'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'

const Shopping: Component = () => {
    const [weatherLocation, setWeatherLocation] = createSignal('London')
    const [weather] = createResource(weatherLocation, location =>
        fetch(
            `https://localhost/api/weatherforecast?location=${location}`
        ).then(res => res.json())
    )

    const [shoppingList, setShoppingList] = createSignal('')
    const [description, setDescription] = createSignal('')
    const [money, setMoney] = createSignal(0)
    const [points, setPoints] = createSignal(0)
    const [allPeople, setAllPeople] = createSignal([
        'Anneke',
        'Olaf',
        'Jente',
        'Joël',
        'Selina',
        'Misha'
    ])
    const [peopleList, setPeopleList] = createSignal<string[]>([])
    const [peopleCountList, setPeopleCountList] = createSignal<{
        [key: string]: number
    }>({})
    const changeCount = (key: string, count: number) => {
        const { [key]: old, ...peeps } = peopleCountList()
        if (old + count > 0) peeps[key] = old + count
        setPeopleCountList(peeps)
        return peopleCountList()[key]
    }
    createEffect(() =>
        setPeopleCountList(old =>
            Object.fromEntries(
                peopleList()
                    .map(key => [key, 1])
                    .concat(
                        Object.entries(old).filter(([key]) =>
                            peopleList().includes(key)
                        )
                    )
            )
        )
    )
    const [transactions, setTransactions] = createSignal([
        {
            to: 'Anneke',
            from: ['Jente', 'Misha', 'Olaf'],
            description: 'Pasta',
            money: 18.45,
            points: 3
        },
        {
            to: 'Olaf',
            from: ['Anneke', 'Olaf', 'Jente', 'Joël', 'Selina', 'Misha'],
            description: 'Huisboodschappen',
            money: 8.29,
            points: 0
        }
    ])

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <TextField class="w-full">
                <TextFieldTextArea
                    value={shoppingList()}
                    onInput={e => setShoppingList(e.currentTarget.value)}
                    class="resize-none h-64"
                    placeholder="Shopping list"
                />
            </TextField>
            <Flex flexDirection="col" class="gap-2 rounded border p-2">
                <FlexRow>
                    <TextField class="w-full">
                        <TextFieldInput
                            type="text"
                            placeholder="New transaction"
                            value={description()}
                            onInput={e => setDescription(e.currentTarget.value)}
                        />
                    </TextField>
                    <Button
                        onClick={() => {
                            fetch('/api/addTransaction', {
                                method: 'POST',
                                body: JSON.stringify({
                                    from: peopleList(),
                                    description: description(),
                                    money: money(),
                                    points: points()
                                })
                            })
                        }}
                    >
                        Add
                    </Button>
                </FlexRow>
                <NumberRow
                    text="Money"
                    value={money()}
                    setValue={setMoney}
                    max={1000}
                    step={0.01}
                />
                <NumberRow
                    text="Points"
                    value={points()}
                    setValue={setPoints}
                />
                <Select<string>
                    multiple
                    value={peopleList()}
                    onChange={setPeopleList}
                    options={allPeople()}
                    placeholder="Add some people"
                    class="w-full"
                    itemComponent={props => (
                        <SelectItem item={props.item}>
                            {props.item.rawValue}
                        </SelectItem>
                    )}
                >
                    <SelectTrigger
                        aria-label="People paying"
                        class="h-fit p-2"
                        as="div"
                    >
                        <SelectValue<string> class="flex gap-2 flex-wrap w-fit">
                            {state => (
                                <Index each={state.selectedOptions()}>
                                    {o => (
                                        <div class="h-fit">
                                            <Button
                                                variant="outline"
                                                class="rounded-full w-6 h-6 p-1"
                                                onClick={() => {
                                                    if (!changeCount(o(), -1))
                                                        state.remove(o())
                                                }}
                                                onPointerDown={(e: Event) =>
                                                    e.stopPropagation()
                                                }
                                                onKeyDown={(e: Event) =>
                                                    e.stopPropagation()
                                                }
                                            >
                                                -
                                            </Button>
                                            <span class="mx-1">
                                                {o()} x{peopleCountList()[o()]}
                                            </span>
                                            <Button
                                                variant="outline"
                                                class="rounded-full w-6 h-6 p-1"
                                                onClick={() => {
                                                    changeCount(o(), 1)
                                                }}
                                                onPointerDown={(e: Event) =>
                                                    e.stopPropagation()
                                                }
                                                onKeyDown={(e: Event) =>
                                                    e.stopPropagation()
                                                }
                                            >
                                                +
                                            </Button>
                                        </div>
                                    )}
                                </Index>
                            )}
                        </SelectValue>
                    </SelectTrigger>
                    <SelectContent />
                </Select>
            </Flex>
            <For each={transactions()}>
                {transaction => (
                    <Card class="p-2 w-full">
                        <CardHeader class="p-0">
                            <CardTitle>
                                {transaction.to} 🍴{transaction.points} 🪙
                                {transaction.money}
                            </CardTitle>
                            <CardDescription>
                                {transaction.description}
                            </CardDescription>
                        </CardHeader>
                        <CardContent class="p-0">
                            {transaction.from.join(', ')}
                        </CardContent>
                    </Card>
                )}
            </For>
        </Flex>
    )
}

export default Shopping
