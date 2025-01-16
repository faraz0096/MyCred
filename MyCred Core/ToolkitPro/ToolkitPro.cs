using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.ToolkitPro
{
    public class ToolkitPro : BaseClass
    {
        [Test, Order(1)]
        public async Task EnableAddons()
        {
            await _page.Locator(MyCredMainMenu).ClickAsync();
            await _page.Locator("//a[normalize-space()='Toolkit']").ClickAsync();

            // Ensure the page is fully loaded
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Get the locator for all checkboxes
            var checkBoxLocator = _page.Locator("//input[@type='checkbox']");

            // Get the count of checkboxes
            int countCheckBox = await checkBoxLocator.CountAsync();
            Console.WriteLine("Number of Addons: " + countCheckBox);

            for (int i = 0; i < countCheckBox; i++)
            {
                // Get the nth checkbox as a locator
                var singleCheckBox = checkBoxLocator.Nth(i);

                // Wait for the specific checkbox to be visible and enabled
                await singleCheckBox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                // Check if the checkbox is already checked
                bool isChecked = await singleCheckBox.IsCheckedAsync();
                if (!isChecked)
                {
                    // Click the checkbox if it is not checked
                    await singleCheckBox.ScrollIntoViewIfNeededAsync();
                    await singleCheckBox.ClickAsync();
             
                }

                Console.WriteLine($"Checkbox {i + 1} checked state: {isChecked}");
            }
        }
    }
}
