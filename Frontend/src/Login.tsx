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
    NumberField,
    NumberFieldDecrementTrigger,
    NumberFieldIncrementTrigger,
    NumberFieldInput
} from '~/components/ui/number-field'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue
} from '~/components/ui/select'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'
import {
    TextField,
    TextFieldInput,
    TextFieldLabel,
    TextFieldErrorMessage
} from '~/components/ui/text-field'
import { Button } from '~/components/ui/button'

import FlexRow from '~/components/FlexRow'
import NumberRow from '~/components/NumberRow'
import { activeResource, pollingResource } from '~/lib/activeResource'

const isValidEmail = (email: string) => /.+@.+\..+/.test(email)
const isValidPassword = (password: string) => /[a-zA-Z]{15,}/.test(password)

const Login: Component = () => {
    const [email, setEmail] = createSignal('')
    const [password, setPassword] = createSignal('')
    const [password2, setPassword2] = createSignal('')

    return (
        <Tabs
            defaultValue="login"
            class="max-w-md h-screen mx-auto p-2 flex flex-col justify-center"
        >
            <TabsList class="grid w-full grid-cols-2">
                <TabsTrigger value="login">Login</TabsTrigger>
                <TabsTrigger value="register">Register</TabsTrigger>
            </TabsList>
            <TabsContent value="login" class="flex flex-col gap-2">
                <TextField
                    class="w-full flex flex-col gap-2"
                    validationState={
                        email().length > 0 && !isValidEmail(email())
                            ? 'invalid'
                            : 'valid'
                    }
                >
                    <TextFieldInput
                        type="email"
                        id="email"
                        placeholder="Email"
                        value={email()}
                        onInput={e => setEmail(e.currentTarget.value)}
                    />
                    <TextFieldErrorMessage>
                        This must be a valid email
                    </TextFieldErrorMessage>
                </TextField>
                <TextField
                    class="w-full flex flex-col gap-2"
                    validationState={
                        password().length > 0 && !isValidPassword(password())
                            ? 'invalid'
                            : 'valid'
                    }
                >
                    <TextFieldInput
                        type="password"
                        id="password"
                        placeholder="Password"
                        value={password()}
                        onInput={e => setPassword(e.currentTarget.value)}
                    />
                    <TextFieldErrorMessage>
                        Your password must be at least 15 characters long. It
                        can only contain letters from a-z. This way you can
                        easily remember it, but it will still be safe.
                    </TextFieldErrorMessage>
                </TextField>
                <Button
                    class="w-full"
                    onClick={() => {
                        if (
                            !isValidEmail(email()) ||
                            !isValidPassword(password())
                        )
                            return
                        fetch('/api/login?useCookies=true', {
                            method: 'POST',
                            body: JSON.stringify({
                                email: email(),
                                password: password()
                            }),
                            headers: {
                                'Content-Type': 'application/json',
                                Accept: 'application/json'
                            }
                        })
                    }}
                >
                    Login
                </Button>
            </TabsContent>
            <TabsContent value="register" class="flex flex-col gap-2">
                <TextField
                    class="w-full flex flex-col gap-2"
                    validationState={
                        email().length > 0 && !isValidEmail(email())
                            ? 'invalid'
                            : 'valid'
                    }
                >
                    <TextFieldInput
                        type="email"
                        id="email"
                        placeholder="Email"
                        value={email()}
                        onInput={e => setEmail(e.currentTarget.value)}
                    />
                    <TextFieldErrorMessage>
                        This must be a valid email
                    </TextFieldErrorMessage>
                </TextField>
                <FlexRow alignItems="start">
                    <TextField
                        class="w-full flex flex-col gap-2"
                        validationState={
                            password().length > 0 &&
                            !isValidPassword(password())
                                ? 'invalid'
                                : 'valid'
                        }
                    >
                        <TextFieldInput
                            type="password"
                            id="password"
                            placeholder="Password"
                            value={password()}
                            onInput={e => setPassword(e.currentTarget.value)}
                        />
                        <TextFieldErrorMessage>
                            Your password must be at least 15 characters long.
                            It can only contain letters from a-z. This way you
                            can easily remember it, but it will still be safe.
                        </TextFieldErrorMessage>
                    </TextField>
                    <TextField
                        class="w-full flex flex-col gap-2"
                        validationState={
                            password2().length > 0 && password2() !== password()
                                ? 'invalid'
                                : 'valid'
                        }
                    >
                        <TextFieldInput
                            type="password"
                            id="password-2"
                            placeholder="Repeat password"
                            value={password2()}
                            onInput={e => setPassword2(e.currentTarget.value)}
                        />
                        <TextFieldErrorMessage>
                            Your passwords must match
                        </TextFieldErrorMessage>
                    </TextField>
                </FlexRow>
                <Button
                    class="w-full"
                    onClick={() => {
                        if (
                            !isValidEmail(email()) ||
                            !isValidPassword(password()) ||
                            password2() !== password()
                        )
                            return
                        fetch('/api/register', {
                            method: 'POST',
                            body: JSON.stringify({
                                email: email(),
                                password: password()
                            }),
                            headers: {
                                'Content-Type': 'application/json',
                                Accept: 'application/json'
                            }
                        }).then(() =>
                            fetch('/api/login?useCookies=true', {
                                method: 'POST',
                                body: JSON.stringify({
                                    email: email(),
                                    password: password()
                                }),
                                headers: {
                                    'Content-Type': 'application/json',
                                    Accept: 'application/json'
                                }
                            })
                        )
                    }}
                >
                    Register
                </Button>
            </TabsContent>
        </Tabs>
    )
}

export default Login
