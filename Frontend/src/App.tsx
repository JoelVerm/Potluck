import type { Component } from 'solid-js';

import logo from './logo.svg';
import styles from './App.module.css';
import { createResource, createSignal, For } from 'solid-js';
import { Flex } from './components/ui/flex';
import { Switch, SwitchControl, SwitchLabel, SwitchThumb } from './components/ui/switch';
import { TextField, TextFieldInput, TextFieldLabel } from './components/ui/text-field';

const App: Component = () => {
  const [weatherLocation, setWeatherLocation] = createSignal('London')
  const [weather] = createResource(weatherLocation, (location) => fetch(`https://localhost/api/weatherforecast?location=${location}`).then(res => res.json()))

  return (
    <Flex flexDirection='col' alignItems='center' justifyContent='center' class="gap-2 my-2">
      <Switch class="flex items-center space-x-2">
        <SwitchControl>
          <SwitchThumb />
        </SwitchControl>
        <SwitchLabel>Test switch</SwitchLabel>
      </Switch>
      <TextField class="grid w-full max-w-sm items-center gap-1.5">
        <TextFieldLabel for="weatherLocation">Weather location</TextFieldLabel>
        <TextFieldInput type="search" id="weatherLocation" placeholder={weatherLocation()} onInput={(e) => setWeatherLocation(e.currentTarget.value)} />
      </TextField>
      <h4>Weather in {weatherLocation()}:</h4>
      <For each={weather()}>
        {(weather) => <p>{weather}</p>}
      </For>
    </Flex>
  );
};

export default App;
