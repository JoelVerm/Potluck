import type {Component, Signal} from 'solid-js'
import {createResource, createSignal, For} from 'solid-js'

import {Button} from '~/components/ui/button'
import {Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger} from '~/components/ui/dialog'
import {Flex} from '~/components/ui/flex'
import {TextField, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import {apiCall, createInitUserListWS, createInitWS} from 'api'

const Settings: Component<{ username_signal: Signal<string> }> = props => {
    const [userName] = props.username_signal

    const [dietPreferences, setDietPreferences] =
        createInitUserListWS('/dietPreferences')
    const [houseName, setHouseName] = createInitWS('/houseName')
    const [houseMembers] = createResource(() => apiCall('/houseMembers', 'get'))

    const [newHouseName, setNewHouseName] = createSignal('')

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
                        dietPreferences().filter(e => e?.User == userName())[0]
                            ?.Value ?? ''
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
                {(houseName()?.length ?? 0) <= 0 ? (
                    <>
                        <TextField class="w-full">
                            <TextFieldInput
                                type="text"
                                placeholder="House name"
                                value={newHouseName()}
                                onInput={e =>
                                    setNewHouseName(e.currentTarget.value)
                                }
                            />
                        </TextField>
                        <Button
                            onClick={() => {
                                if ((newHouseName()?.length ?? 0) > 0) {
                                    apiCall(
                                        '/createHouse',
                                        'post',
                                        undefined,
                                        newHouseName()
                                    )
                                }
                            }}
                        >
                            Create house
                        </Button>
                    </>
                ) : (
                    <>
                        <TextField class="w-full">
                            <TextFieldInput
                                type="text"
                                placeholder="House name"
                                value={houseName()}
                                onInput={e =>
                                    setHouseName(e.currentTarget.value)
                                }
                            />
                        </TextField>
                        <For each={houseMembers()}>
                            {member => (
                                <FlexRow>
                                    <span>{member}</span>
                                    <Button
                                        onClick={() => {
                                            apiCall(
                                                '/removeHouseMember',
                                                'post',
                                                undefined,
                                                member
                                            )
                                        }}
                                    >
                                        Remove
                                    </Button>
                                </FlexRow>
                            )}
                        </For>
                        <AddUserDialog/>
                    </>
                )}
            </Flex>
        </Flex>
    )
}

const AddUserDialog: Component = () => {
    const [open, setOpen] = createSignal(false)
    const [userName, setUserName] = createSignal('')

    const addMember = () => {
        apiCall('/addHouseMember', 'post', undefined, userName())
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
