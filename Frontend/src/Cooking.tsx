import {Component, createEffect, createResource, createSignal, Index, Show} from 'solid-js'
import {Flex} from '~/components/ui/flex'
import {Switch, SwitchControl, SwitchThumb} from '~/components/ui/switch'
import {TextField, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import {client, createWS, readOnlyWS} from 'api'
import {TabProps} from "~/App";

interface EatingPerson {
    name: string
    count: number
    cookingPoints: number
    diet: string
}

const Cooking: Component<TabProps> = props => {
    const [cookingUser, setCooking] = createWS('/houses/{name}/cookingUserWS', () => ({
        name: props.houseName
    }))
    const [dinnerInfo, {refetch}] = createResource(async () => {
        if (props.houseName.length <= 0) return undefined
        const res = await client.GET('/houses/{name}/dinner', {
            params: {
                path: {
                    name: props.houseName
                }
            }
        })
        return res.data
    })
    const setCookingInfo = (price: number | undefined, description: string | undefined) => {
        if (props.houseName.length <= 0) return
        client.PUT('/houses/{name}/dinner', {
            params: {
                path: {
                    name: props.houseName
                }
            },
            body: {
                price: price ?? dinnerInfo()?.price ?? 0,
                description: description ?? dinnerInfo()?.description ?? ''
            }
        }).then(() => refetch())
    }
    const setCookingTotal = (price: number) => setCookingInfo(price, undefined)
    const setDescription = (description: string) => setCookingInfo(undefined, description)
    const [eatingList, setEatingList] = createSignal<
        EatingPerson[]
    >([])
    createEffect(async () => {
        if (props.houseName.length <= 0) return
        const res = await client.GET('/houses/{name}/users/eating', {
            params: {
                path: {
                    name: props.houseName
                }
            }
        })
        if (res.data != undefined)
            setEatingList(res.data.eatingList as EatingPerson[])
    })
    createEffect(async () => {
        eatingList().forEach(p => {
            readOnlyWS('/users/{name}/eatingTotalPeopleWS', () => ({
                name: p.name
            }), (total) => {
                if (total != undefined && total != p.count) {
                    p.count = total ?? 0
                    setEatingList(eatingList().filter(u => u.count > 0))
                }
            })
        })
    })

    const cooking = () => cookingUser() == props.username

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <Show
                when={cooking() || ((cookingUser()?.trim() ?? "") == "")}
                fallback={<h1>{cookingUser()} is cooking
                    today!</h1>}
            >
                <FlexRow>
                    <span>I'm cooking today!</span>
                    <Switch
                        class="flex items-center space-x-2"
                        checked={cooking()}
                        onChange={setCooking}
                    >
                        <SwitchControl>
                            <SwitchThumb/>
                        </SwitchControl>
                    </Switch>
                </FlexRow>
                <NumberRow
                    text="Cooking total"
                    value={dinnerInfo()?.price ?? 0}
                    setValue={setCookingTotal}
                    step={0.01}
                    enabled={cooking()}
                />
                <TextField class="w-full">
                    <TextFieldInput
                        type="text"
                        placeholder="Meal description"
                        value={dinnerInfo()?.description ?? ''}
                        onInput={e => setDescription(e.currentTarget.value)}
                        disabled={!cooking()}
                    />
                </TextField>
            </Show>
            <h1>
                {eatingList().reduce((t, p) => t + p.count, 0) ?? 0} people
                eating today
            </h1>
            <Index
                each={eatingList().toSorted((a, b) =>
                    a.name.localeCompare(b.name)
                )}
            >
                {person => (
                    <div class="my-2">
                        <h1>
                            {person().name} 🍴{person().cookingPoints}
                        </h1>
                        {person().count > 1 ? (
                            <p class="text-muted-foreground">
                                Takes {person().count - 1} friends
                            </p>
                        ) : (
                            ''
                        )}
                        <p class="text-muted-foreground">{person().diet}</p>
                    </div>
                )}
            </Index>
        </Flex>
    )
}

export default Cooking
