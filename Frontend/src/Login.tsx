import type {Component} from 'solid-js'
import {createSignal} from 'solid-js'
import {Button} from '~/components/ui/button'
import {Tabs, TabsContent, TabsList, TabsTrigger} from '~/components/ui/tabs'
import {TextField, TextFieldErrorMessage, TextFieldInput} from '~/components/ui/text-field'

import FlexRow from '~/components/FlexRow'
import {client} from "api";

const isValidEmail = (email: string) => /.+@.+\..+/.test(email)
const isValidPassword = (password: string) =>
    password.length >= 15 &&
    password.match(/[a-z]/) &&
    password.match(/[A-Z]/) &&
    !password.match(/[^a-zA-Z]/)

const setUsernameCookie = (name: string) => {
    document.cookie = `username=${name}`
}

export const getUsernameCookie = () => {
    return document.cookie.split('; ').find(row => row.startsWith('username='))?.split('=')[1]
}

const Login: Component<{ setUsername: (name: string) => void }> = props => {
    const [email, setEmail] = createSignal('')
    const [password, setPassword] = createSignal('')
    const [password2, setPassword2] = createSignal('')
    const [hasError, setHasError] = createSignal(false)

    return (
        <Tabs
            defaultValue="login"
            class="max-w-md h-screen mx-auto p-2 flex flex-col justify-center"
        >
            <TabsList class="grid w-full grid-cols-2">
                <TabsTrigger value="login" onClick={() => setHasError(false)}>Login</TabsTrigger>
                <TabsTrigger value="register" data-testid="register-tab"
                             onClick={() => setHasError(false)}>Register</TabsTrigger>
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
                        This password is invalid
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
                        client.POST('/login', {
                            params: {
                                query: {
                                    useCookies: true
                                }
                            },
                            body: {
                                email: email(),
                                password: password()
                            }
                        }).then(res => {
                            setHasError(res.error != undefined)
                            if (res.error != undefined) return
                            setUsernameCookie(email())
                            props.setUsername(email())
                        })
                    }}
                >
                    Login
                </Button>
                <p class="text-red-500">{hasError() ? 'Could not login' : ''}</p>
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
                        data-testid="register-email"
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
                            data-testid="register-password"
                        />
                        <TextFieldErrorMessage>
                            Your password must be at least 15 characters long.
                            It can only contain letters from a-z and has to
                            contain both lowercase and uppercase letters. This
                            way you can easily remember it, but it will still be
                            safe.
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
                            data-testid="register-password-2"
                        />
                        <TextFieldErrorMessage>
                            Your passwords must match
                        </TextFieldErrorMessage>
                    </TextField>
                </FlexRow>
                <Button
                    class="w-full"
                    data-testid="register-button"
                    onClick={() => {
                        if (
                            !isValidEmail(email()) ||
                            !isValidPassword(password()) ||
                            password2() !== password()
                        )
                            return
                        client.POST(
                            '/register',
                            {
                                body: {
                                    email: email(),
                                    password: password()
                                }
                            }
                        )
                            .then(() =>
                                client.POST(
                                    '/login',
                                    {
                                        params: {
                                            query: {
                                                useCookies: true
                                            }
                                        },
                                        body: {
                                            email: email(),
                                            password: password()
                                        }
                                    }
                                )
                            )
                            .then(res => {
                                setHasError(res.error != undefined)
                                if (res.error != undefined) return
                                setUsernameCookie(email())
                                props.setUsername(email())
                            })
                    }}
                >
                    Register
                </Button>
                <p class="text-red-500">{hasError() ? 'Could not register' : ''}</p>
            </TabsContent>
        </Tabs>
    )
}

export default Login
