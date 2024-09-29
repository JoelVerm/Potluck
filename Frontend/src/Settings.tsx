import type { Component } from 'solid-js'

import {
    createResource,
    createSignal,
    createEffect,
    For,
    Index,
    createMemo
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
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle
} from '~/components/ui/card'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from '~/components/ui/select'
import { Separator } from '~/components/ui/separator'
import { Button } from '~/components/ui/button'
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger
} from '~/components/ui/dialog'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import { activeResource, pollingResource } from '~/lib/activeResource'

const Settings: Component = () => {
    const [dietPreferences, setDietPreferences] = activeResource<string>(
        '/api/dietPreferences'
    )
    const [houseName, setHouseName] = activeResource<string>('/api/houseName')
    const [houseMembers] = pollingResource<string[]>('/api/houseMembers')

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
                    value={dietPreferences()}
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
                                    fetch('/api/createHouse', {
                                        method: 'POST',
                                        body: JSON.stringify(newHouseName()),
                                        headers: {
                                            'Content-Type': 'application/json',
                                            Accept: 'application/json'
                                        }
                                    })
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
                                            fetch('/api/removeHouseMember', {
                                                method: 'POST',
                                                body: JSON.stringify(member),
                                                headers: {
                                                    'Content-Type':
                                                        'application/json',
                                                    Accept: 'application/json'
                                                }
                                            })
                                        }}
                                    >
                                        Remove
                                    </Button>
                                </FlexRow>
                            )}
                        </For>
                        <AddUserDialog />
                    </>
                )}
            </Flex>
        </Flex>
    )
}

const AddUserDialog: Component = () => {
    const [open, setOpen] = createSignal(false)
    const [userName, setUserName] = createSignal('')

    return (
        <Dialog open={open()} onOpenChange={setOpen}>
            <DialogTrigger as={Button}>Add house member</DialogTrigger>
            <DialogContent class="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Add house member</DialogTitle>
                </DialogHeader>
                <TextField class="grid grid-cols-4 items-center gap-4">
                    <TextFieldLabel class="text-right">Name</TextFieldLabel>
                    <TextFieldInput
                        value={userName()}
                        onInput={e => setUserName(e.currentTarget.value)}
                        type="text"
                    />
                </TextField>
                <DialogFooter>
                    <Button
                        type="submit"
                        onClick={() => {
                            fetch('/api/addHouseMember', {
                                method: 'POST',
                                body: JSON.stringify(userName()),
                                headers: {
                                    'Content-Type': 'application/json',
                                    Accept: 'application/json'
                                }
                            })
                            setOpen(false)
                        }}
                    >
                        Add
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default Settings
