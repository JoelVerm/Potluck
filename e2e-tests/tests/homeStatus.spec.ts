import {expect, test} from '@playwright/test';
import {auth, USERNAME} from "./auth";

test('Correctly displaying eating total', async ({page}) => {
    await auth(page);
    await page.getByTestId('tab-Home').click();
    await page.getByTestId('homeStatus-button').click();
    await page.getByTestId('homeStatus-options').getByText("Out of town").click();
    await expect(page.getByTestId('homeStatus-Out-of-town')).toContainText(USERNAME);
});
