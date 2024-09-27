import { Accessor, Component, Setter, Signal, mergeProps } from 'solid-js'
import { Flex, FlexProps } from '~/components/ui/flex'

const FlexRow: Component<FlexProps> = props => {
    const finalProps = mergeProps(
        {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'between'
        } satisfies FlexProps,
        props
    )

    return <Flex {...finalProps} class="gap-2"></Flex>
}

export default FlexRow
