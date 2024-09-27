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

interface EatingPerson {
    name: string
    count: number
    cookingPoints: number
    diet: string
}

const Cooking: Component = () => {
    const [weatherLocation, setWeatherLocation] = createSignal('London')
    const [weather] = createResource(weatherLocation, location =>
        fetch(
            `https://localhost/api/weatherforecast?location=${location}`
        ).then(res => res.json())
    )

    const [cooking, setCooking] = createSignal(false)
    const [cookingTotal, setCookingTotal] = createSignal(0)
    const [description, setDescription] = createSignal('')
    const [eatingList, _] = createSignal<EatingPerson[]>([
        { name: 'Jente', count: 2, cookingPoints: 12, diet: '' },
        { name: 'Anneke', count: 1, cookingPoints: 0, diet: 'Vegetarian' },
        { name: 'Joël', count: 3, cookingPoints: -3, diet: 'Vegetarian' },
        { name: 'Olaf', count: 1, cookingPoints: 3, diet: '' },
        { name: 'Selina', count: 1, cookingPoints: -8, diet: 'Vegetarian' },
        { name: 'Misha', count: 1, cookingPoints: 1, diet: 'Less spicy' }
    ])

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
                    defaultChecked={cooking()}
                    onChange={setCooking}
                >
                    <SwitchControl>
                        <SwitchThumb />
                    </SwitchControl>
                </Switch>
            </FlexRow>
            <NumberRow
                text="Cooking total"
                value={cookingTotal()}
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
                    {eatingList().reduce((t, p) => t + p.count, 0)} people
                    eating today
                </h1>
                <For
                    each={eatingList().toSorted((a, b) =>
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
