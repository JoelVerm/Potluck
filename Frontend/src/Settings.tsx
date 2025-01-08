import {Component, createResource, createSignal, For} from 'solid-js'

import {Button} from '~/components/ui/button'
import {Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger} from '~/components/ui/dialog'
import {Flex} from '~/components/ui/flex'
import {TextField, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import {client, createWS} from 'api'
import {TabProps} from "~/App";

const Settings: Component<TabProps> = props => {
    const [dietPreferences, setDietPreferences] =
        createWS('/users/{name}/dietWS', () => ({name: props.username}))
    const [houseMembers, {refetch}] = createResource(async () => {
        if (props.houseName.length <= 0) return undefined
        const res = await client.GET('/houses/{name}/users', {
            params: {
                path: {
                    name: props.houseName
                }
            }
        })
        return res.data
    })

    return (
        <Flex
            flexDirection="col"
            alignItems="start"
            justifyContent="center"
            class="gap-2"
        >
            <TextField class="w-full">
                <TextFieldInput
                    type="text"
                    placeholder="Diet info"
                    value={
                        dietPreferences() ?? ''
                    }
                    onInput={e => setDietPreferences(e.currentTarget.value)}
                />
            </TextField>
            <Flex
                flexDirection="col"
                justifyContent="start"
                alignItems="start"
                class="gap-2 rounded border p-2"
            >
                <h2 class="text-lg">House members of {props.houseName}</h2>
                <For each={houseMembers()?.names}>
                    {member => (
                        <FlexRow>
                            <span>{member.name}</span>
                            <Button
                                onClick={async () => {
                                    if (props.houseName.length <= 0) return
                                    await client.DELETE(
                                        '/houses/{name}/users/{username}',
                                        {
                                            params: {
                                                path: {
                                                    name: props.houseName,
                                                    username: member.name ?? ""
                                                }
                                            }
                                        }
                                    )
                                    refetch()
                                }}
                            >
                                Remove
                            </Button>
                        </FlexRow>
                    )}
                </For>
                <AddUserDialog {...props} refetch={refetch}/>
            </Flex>
        </Flex>
    )
}

const AddUserDialog: Component<TabProps & { refetch: () => void }> = props => {
    const [open, setOpen] = createSignal(false)
    const [userName, setUserName] = createSignal('')

    const addMember = async () => {
        await client.POST('/houses/{name}/users', {
            params: {
                path: {
                    name: props.houseName
                }
            },
            body: {
                name: userName()
            }
        })
        setOpen(false)
        props.refetch()
    }

    return (
        <Dialog open={open()} onOpenChange={setOpen}>
            <DialogTrigger as={Button}>Add house member</DialogTrigger>
            <DialogContent class="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Add house member</DialogTitle>
                </DialogHeader>
                <FlexRow>
                    <TextField class="w-full">
                        <TextFieldInput
                            value={userName()}
                            onInput={e => setUserName(e.currentTarget.value)}
                            onKeyDown={(e: KeyboardEvent) => {
                                if (e.key === 'Enter') addMember()
                            }}
                            type="text"
                            placeholder="E-mail"
                        />
                    </TextField>
                    <Button type="submit" onClick={addMember}>
                        Add
                    </Button>
                </FlexRow>
            </DialogContent>
        </Dialog>
    )
}

export default Settings
