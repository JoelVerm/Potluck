import {Component, createEffect, createSignal, Index, Signal, untrack} from 'solid-js'
import {Flex} from '~/components/ui/flex'
import {Switch, SwitchControl, SwitchThumb} from '~/components/ui/switch'
import {TextField, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import {createInitUserListWS, createInitWS} from 'api'
import {apiCall, createGetPutResource} from 'api/api'

interface EatingPerson {
    name: string
    count: number
    cookingPoints: number
    diet: string
}

const Cooking: Component<{ username_signal: Signal<string> }> = props => {
    const [username] = props.username_signal
    const [cookingUser, setCooking] = createInitWS('/houses/current/users/cooking')
    const [cookingTotal, setCookingTotal] =
        createGetPutResource('/houses/current/dinner/price')
    const [description, setDescription] = createGetPutResource(
        '/houses/current/dinner/description'
    )
    const [eatingList, setEatingList] = createSignal<
        EatingPerson[]
    >([])
    apiCall('/houses/current/users/eating', 'get').then(setEatingList)
    const [eatingTotalUsers] = createInitUserListWS('/users/current/eatingTotalPeople')
    createEffect(() => {
        const eating = untrack(eatingList)
        const eatingListNames = eating?.map(p => p.name)
        const everyUserIncluded = eatingTotalUsers().map(u => u?.user).every(u => eatingListNames?.includes(u ?? ""))
        if (!everyUserIncluded) {
            apiCall('/houses/current/users/eating', 'get').then(setEatingList)
            return
        }
        setEatingList(eating?.map(p => (
                {
                    ...p,
                    count: eatingTotalUsers().find(u => u?.user == p.name)?.value ?? 0
                }
            )
        ).filter(p => p.count > 0))
    })

    const cooking = () => cookingUser() == username()

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            {cooking() || ((cookingUser()?.trim() ?? "") == "") ? (
                <>
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
                        value={cookingTotal() ?? 0}
                        setValue={setCookingTotal}
                        step={0.01}
                        enabled={cooking()}
                    />
                    <TextField class="w-full">
                        <TextFieldInput
                            type="text"
                            placeholder="Meal description"
                            value={description()}
                            onInput={e => setDescription(e.currentTarget.value)}
                            disabled={!cooking()}
                        />
                    </TextField>
                </>
            ) : (
                <h1>{cookingUser()} is cooking today!</h1>
            )}
            <h1>
                {eatingList()?.reduce((t, p) => t + p.count, 0) ?? 0} people
                eating today
            </h1>
            <Index
                each={eatingList()?.toSorted((a, b) =>
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
