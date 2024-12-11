import {expect, test} from '@playwright/test';

test('Correctly displaying eating total', async ({page}) => {
    await page.goto('http://localhost/');
    await page.getByTestId('tab-Home').click();
    await page.getByTestId('eating-total').getByRole('spinbutton').fill('2');
    await page.getByTestId('tab-Cooking').click();
    await expect(page.getByTestId('eating-total-display')).toContainText('2');
});
