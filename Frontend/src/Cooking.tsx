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

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import { activeResource, pollingResource } from '~/lib/activeResource'

interface EatingPerson {
    name: string
    count: number
    cookingPoints: number
    diet: string
}

const Cooking: Component = () => {
    const [cooking, setCooking] = activeResource<boolean>('/api/cooking')
    const [cookingTotal, setCookingTotal] =
        activeResource<number>('/api/cookingTotal')
    const [description, setDescription] = activeResource<string>(
        '/api/cookingDescription'
    )
    const [eatingList] = pollingResource<EatingPerson[]>('/api/eatingList')

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <FlexRow>
                <span>I'm cooking today!</span>
                <Switch
                    class="flex items-center space-x-2"
                    checked={cooking()}
                    onChange={setCooking}
                >
                    <SwitchControl>
                        <SwitchThumb />
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
            <div>
                <h1>
                    {eatingList()?.reduce((t, p) => t + p.count, 0) ?? 0} people
                    eating today
                </h1>
                <For
                    each={eatingList()?.toSorted((a, b) =>
                        a.name.localeCompare(b.name)
                    )}
                >
                    {person => (
                        <div class="my-2">
                            <h1>
                                {person.name} 🍴{person.cookingPoints}
                            </h1>
                            {person.count > 1 ? (
                                <p class="text-muted-foreground">
                                    Takes {person.count - 1} friends
                                </p>
                            ) : (
                                ''
                            )}
                            <p class="text-muted-foreground">{person.diet}</p>
                        </div>
                    )}
                </For>
            </div>
        </Flex>
    )
}

export default Cooking
