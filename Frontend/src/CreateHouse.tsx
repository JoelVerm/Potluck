import type {Component} from 'solid-js'
import {createSignal} from 'solid-js'
import {Button} from '~/components/ui/button'
import {TextField, TextFieldInput} from '~/components/ui/text-field'
import {client} from "api";
import {Flex} from "~/components/ui/flex";

const CreateHouse: Component<{ setHouseName: (name: string) => void }> = props => {
    const [newHouseName, setNewHouseName] = createSignal('')

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2 max-w-lg mx-auto my-16"
        >
            <h1 class='font-medium'>Add your own house here or come back after someone added you!</h1>
            <TextField class="w-full">
                <TextFieldInput
                    type="text"
                    placeholder="House name"
                    value={newHouseName()}
                    data-testid="new-house-name"
                    onInput={e =>
                        setNewHouseName(e.currentTarget.value)
                    }
                />
            </TextField>
            <Button
                data-testid="new-house"
                onClick={() => {
                    if ((newHouseName()?.length ?? 0) > 0) {
                        client.POST(
                            '/houses',
                            {
                                body: {
                                    name: newHouseName()
                                }
                            }
                        ).then(res => {
                            if (res.response.ok) {
                                props.setHouseName(newHouseName())
                            }
                        })
                    }
                }}
            >
                Create house
            </Button>
        </Flex>
    )
}

export default CreateHouse
