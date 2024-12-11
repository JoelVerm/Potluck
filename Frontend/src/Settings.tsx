import {Component, createEffect, createResource, createSignal, For, Show} from 'solid-js'

import {Button} from '~/components/ui/button'
import {Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger} from '~/components/ui/dialog'
import {Flex} from '~/components/ui/flex'
import {TextField, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import {client, createWS} from 'api'
import {TabProps} from "~/App";

const Settings: Component<TabProps & { setHouseName: (name: string) => void }> = props => {
    const [dietPreferences, setDietPreferences] =
        createWS('/users/{name}/dietWS', () => ({name: props.username}))
    const [houseNameWS, setHouseNameWS] = createWS('/houses/{name}/nameWS', () => ({
        name: props.houseName
    }))
    createEffect(() => {
        props.setHouseName(houseNameWS()?.toString() ?? '')
    })
    const [houseMembers] = createResource(async () => {
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

    const [newHouseName, setNewHouseName] = createSignal(props.houseName)

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
                <Show when={(houseNameWS()?.length ?? 0) > 0} fallback={<>
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
                </>}>
                    <TextField class="w-full">
                        <TextFieldInput
                            type="text"
                            placeholder="House name"
                            value={houseNameWS()}
                            onInput={e =>
                                setHouseNameWS(e.currentTarget.value)
                            }
                        />
                    </TextField>
                    <For each={houseMembers()?.names}>
                        {member => (
                            <FlexRow>
                                <span>{member.name}</span>
                                <Button
                                    onClick={() => {
                                        if (props.houseName.length <= 0) return
                                        client.DELETE(
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
                                    }}
                                >
                                    Remove
                                </Button>
                            </FlexRow>
                        )}
                    </For>
                    <AddUserDialog {...props}/>
                </Show>
            </Flex>
        </Flex>
    )
}

const AddUserDialog: Component<TabProps> = props => {
    const [open, setOpen] = createSignal(false)
    const [userName, setUserName] = createSignal('')

    const addMember = () => {
        client.POST('/houses/{name}/users', {
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
