import {expect, test} from '@playwright/test';
import * as path from "path";

const authFile = path.join(__dirname, '../playwright/.cache/login.json');

test('Be able to register', async ({page}) => {
    const FIVE_MINUTES_MS = 5 * 60 * 1000;
    await page.goto('http://localhost/', {timeout: FIVE_MINUTES_MS});
    if (await page.getByTestId('tab-Home').isVisible()) return;
    await page.getByTestId('register-tab').click({timeout: FIVE_MINUTES_MS});
    await page.getByTestId('register-email').fill('a@b.c');
    await page.getByTestId('register-password').fill('AbcAbcAbcAbcAbc');
    await page.getByTestId('register-password-2').fill('AbcAbcAbcAbcAbc');
    await page.getByTestId('register-button').click();
    await page.getByTestId('tab-Settings').click();
    await page.getByTestId('new-house-name').fill('TestHouse');
    await page.getByTestId('new-house').click();
    await expect(page.getByTestId('edit-house-name')).toBeVisible({timeout: FIVE_MINUTES_MS});
    await page.context().storageState({path: authFile});
});
