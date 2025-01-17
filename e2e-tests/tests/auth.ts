import {expect, Page, test} from '@playwright/test';

export const USERNAME = 'a@b.c';
export const PASSWORD = 'AbcAbcAbcAbcAbc';

export const auth = async (page: Page) => {
    const TIMEOUT = 10 * 60 * 1000; // 10 minutes
    test.setTimeout(TIMEOUT);
    await page.goto('http://localhost/', {timeout: TIMEOUT});
    if (await page.getByTestId('tab-Home').isVisible()) return;
    await page.getByTestId('register-tab').click({timeout: TIMEOUT});
    await page.getByTestId('register-email').fill(USERNAME);
    await page.getByTestId('register-password').fill(PASSWORD);
    await page.getByTestId('register-password-2').fill(PASSWORD);
    let register_response = page.waitForResponse('**/register**', {timeout: TIMEOUT});
    await page.getByTestId('register-button').click();
    await register_response;
    await page.waitForTimeout(100);
    let register_error = await page.getByTestId('register-error').textContent();
    if ((register_error?.length ?? 0) > 0) {
        await page.getByTestId('login-tab').click({timeout: TIMEOUT});
        await page.getByTestId('login-email').fill(USERNAME);
        await page.getByTestId('login-password').fill(PASSWORD);
        let login_response = page.waitForResponse('**/login**', {timeout: TIMEOUT});
        await page.getByTestId('login-button').click();
        await login_response;
        await page.waitForTimeout(100);
    }
    if (await page.getByTestId('tab-Home').isVisible()) return;
    await page.getByTestId('new-house-name').fill('TestHouse');
    await page.getByTestId('new-house').click();
    await expect(page.getByTestId('tab-Home')).toBeVisible({timeout: TIMEOUT});
}