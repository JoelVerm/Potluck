import {expect, test} from '@playwright/test';
import {auth} from "./auth";

test('Correctly displaying eating total', async ({page}) => {
    await auth(page);
    await page.getByTestId('tab-Cooking').click();
    await page.getByTestId('cooking-switch').first().click();
    await page.getByTestId('cooking-total').getByRole("spinbutton").fill("12.34");
    await page.getByTestId('cooking-description').fill("Test description");
    await page.click('body');
    await page.waitForTimeout(1000);
    await expect(page.getByTestId('cooking-total').getByRole("spinbutton")).toHaveValue("12.34");
    await expect(page.getByTestId('cooking-description')).toHaveValue("Test description");
});
