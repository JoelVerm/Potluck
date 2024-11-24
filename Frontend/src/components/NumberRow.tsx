import type {Component} from 'solid-js'

import {
    NumberField,
    NumberFieldDecrementTrigger,
    NumberFieldIncrementTrigger,
    NumberFieldInput
} from '~/components/ui/number-field'

import FlexRow from '~/components/FlexRow'

const NumberRow: Component<{
    text: string
    value: number
    setValue: (value: number) => void
    min?: number
    max?: number
    step?: number
    enabled?: boolean
}> = props => {
    return (
        <FlexRow>
            <span>{props.text}</span>
            <NumberField
                class="w-36"
                onRawValueChange={props.setValue}
                rawValue={props.value}
                minValue={props.min ?? 0}
                maxValue={props.max ?? 100}
                step={props.step ?? 1.0}
                disabled={!(props.enabled ?? true)}
            >
                <div class="relative">
                    <NumberFieldInput/>
                    <NumberFieldIncrementTrigger/>
                    <NumberFieldDecrementTrigger/>
                </div>
            </NumberField>
        </FlexRow>
    )
}

export default NumberRow
