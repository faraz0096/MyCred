using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.NUnitTestCases
{
    public class PntsForEachOrder : BaseClass
    {
        protected string MyCredWooCommerce => _config.GetConfigValue("MyCredWooCommerceSettings");
        protected string RewardSettingsAccordion => _config.GetConfigValue("RewardSettingsAccordion");
        protected string SingleProdToggle => _config.GetConfigValue("SingleProductRewardToggle");

        [Test, Order(1)]
        public async Task RewardSettings()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.Locator(MyCredMainMenu).ClickAsync();
            await _page.Locator(MyCredWooCommerce).ClickAsync();
            await _page.Locator(RewardSettingsAccordion).WaitForAsync();
            await _page.Locator(RewardSettingsAccordion).ClickAsync();

            await _page.Locator(SingleProdToggle).WaitForAsync();
            bool pointsToggleBtn = await _page.Locator(SingleProdToggle).IsCheckedAsync();
            Console.WriteLine(pointsToggleBtn);

            if (pointsToggleBtn == false)
            {
                await _page.Locator(SingleProdToggle).ClickAsync();
            }


            var selectBoxCheckEmpty = await _page.Locator("span[class='select2-selection select2-selection--multiple']").TextContentAsync();
            if (selectBoxCheckEmpty == "")
            {

                await _page.Locator(".select2-selection.select2-selection--multiple").ClickAsync();

                while (await _page.Locator("ul[class='select2-results__options'] li").CountAsync() < 0)
                {

                    // Always target the first matching button dynamically
                    await _page.Locator("ul[class='select2-results__options'] li").ClickAsync();

                }

            }
            /* else
             {

                 await _page.Locator("ul[id='select2-woocommercerewardstatus-results'] li").First.ClickAsync();
             }*/

            var getStatus = await _page.Locator("span[class='select2-selection__choice__display']").AllAsync();

            foreach (var statusText in getStatus)
            {
                var test = await statusText.TextContentAsync();
                Console.WriteLine(test);
            }




            await _page.Locator("//input[@name='submit']").ClickAsync();

        }
    }
}
