import {Component, mergeProps} from 'solid-js'
import {Flex, FlexProps} from '~/components/ui/flex'

const FlexRow: Component<FlexProps> = props => {
    const finalProps = mergeProps(
        {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'between'
        } satisfies FlexProps,
        props
    )

    return <Flex {...finalProps} class="gap-2 w-full"></Flex>
}

export default FlexRow
