import {expect, test} from '@playwright/test';

test('Be able to register', async ({page}) => {
    await page.goto('http://localhost/');
    await page.getByTestId('register-tab').click();
    await page.getByTestId('register-email').fill('a@b.c');
    await page.getByTestId('register-password').fill('AbcAbcAbcAbcAbc');
    await page.getByTestId('register-password-2').fill('AbcAbcAbcAbcAbc');
    await page.getByTestId('register-button').click();
    expect(page.getByTestId('tab-Home')).toBeDefined();
});
