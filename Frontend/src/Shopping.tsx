import type {Component} from 'solid-js'
import {createEffect, createResource, createSignal, For, Index} from 'solid-js'

import {Button} from '~/components/ui/button'
import {Card, CardContent, CardDescription, CardHeader, CardTitle} from '~/components/ui/card'
import {Flex} from '~/components/ui/flex'
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from '~/components/ui/select'
import {TextField, TextFieldInput, TextFieldTextArea} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import {client, createWS} from 'api'
import {TabProps} from "~/App";

const Shopping: Component<TabProps> = props => {
    const [shoppingList, setShoppingList] = createWS(
        `/houses/{name}/shoppingListWS`,
        () => ({name: props.houseName})
    )
    const [allPeople] = createResource(async () => {
        if (props.houseName.length <= 0) return undefined
        const res = await client.GET(`/houses/{name}/users`, {
            params: {
                path: {
                    name: props.houseName
                }
            }
        })
        return res.data
    })

    const [description, setDescription] = createSignal('')
    const [money, setMoney] = createSignal(0)
    const [points, setPoints] = createSignal(0)
    const [peopleList, setPeopleList] = createSignal<string[]>([])
    const [peopleCountList, setPeopleCountList] = createSignal<{
        [key: string]: number
    }>({})
    const changeCount = (key: string, count: number) => {
        const {[key]: old, ...peeps} = peopleCountList()
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
    const [transactions, {refetch}] = createResource(async () => {
            if (props.houseName.length <= 0) return undefined
            const res = await client.GET("/houses/{name}/transactions", {
                params: {
                    path: {
                        name: props.houseName
                    }
                }
            })
            return res.data
        }
    )

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
                        onClick={async () => {
                            if (props.houseName.length <= 0) return
                            await client.POST(
                                '/houses/{name}/transactions',
                                {
                                    params: {
                                        path: {
                                            name: props.houseName
                                        }
                                    },
                                    body:
                                        {
                                            from: Object.entries(
                                                peopleCountList()
                                            ).reduce<string[]>(
                                                (from, v) => [
                                                    ...from,
                                                    ...Array(v[1]).fill(v[0])
                                                ],
                                                []
                                            ),
                                            description: description(),
                                            money: money(),
                                            points: points()
                                        }
                                }
                            )
                            await refetch()
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
                    options={allPeople()?.names?.map(n => n.name ?? "") ?? []}
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
                    <SelectContent/>
                </Select>
            </Flex>
            <For each={transactions()?.transactions}>
                {transaction => (
                    <Card class="p-2 w-full">
                        <CardHeader class="p-0">
                            <CardTitle>
                                {transaction.toUser} 🍴{transaction.cookingPoints} 🪙
                                {transaction.euroCents! / 100}
                            </CardTitle>
                            <CardDescription>
                                {transaction.description}
                            </CardDescription>
                        </CardHeader>
                        <CardContent class="p-0">
                            {transaction.fromUsers?.join(', ')}
                        </CardContent>
                    </Card>
                )}
            </For>
        </Flex>
    )
}

export default Shopping
