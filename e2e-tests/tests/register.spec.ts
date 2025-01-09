import {expect, test} from '@playwright/test';
import * as path from "path";

const authFile = path.join(__dirname, '../playwright/.cache/register.json');

test('Be able to register', async ({page}) => {
    const TIMEOUT = 15 * 60 * 1000;
    test.setTimeout(TIMEOUT);
    await page.goto('http://localhost/', {timeout: TIMEOUT});
    if (await page.getByTestId('tab-Home').isVisible()) return;
    await page.getByTestId('register-tab').click({timeout: TIMEOUT});
    await page.getByTestId('register-email').fill('a@b.c');
    await page.getByTestId('register-password').fill('AbcAbcAbcAbcAbc');
    await page.getByTestId('register-password-2').fill('AbcAbcAbcAbcAbc');
    let register_response = page.waitForResponse('**/register**', {timeout: TIMEOUT});
    await page.getByTestId('register-button').click();
    await register_response;
    await page.waitForTimeout(100);
    let register_error = await page.getByTestId('register-error').textContent();
    if (register_error.length > 0) {
        await page.getByTestId('login-tab').click({timeout: TIMEOUT});
        await page.getByTestId('login-email').fill('a@b.c');
        await page.getByTestId('login-password').fill('AbcAbcAbcAbcAbc');
        let login_response = page.waitForResponse('**/login**', {timeout: TIMEOUT});
        await page.getByTestId('login-button').click();
        await login_response;
        await page.waitForTimeout(100);
    }
    if (await page.getByTestId('tab-Home').isVisible()) return;
    await page.getByTestId('new-house-name').fill('TestHouse');
    await page.getByTestId('new-house').click();
    await expect(page.getByTestId('tab-Home')).toBeVisible({timeout: TIMEOUT});
    await page.context().storageState({path: authFile});
});