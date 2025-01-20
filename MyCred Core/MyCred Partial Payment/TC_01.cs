using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.MyCred_Partial_Payment
{
    public class TC_01 : BaseClass
    {
        [Test,Order(1)]
        public async Task VerifyPointTypeDropDownInDifferentTemplate()
        {
            //TC_01 Verify Point Type Drop Down in Multiple Template (In Cart) Dropdown
            //https://objectsws.atlassian.net/browse/MCD-1014
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            //Set Partial Payment Settings
            await _page.Locator(MyCredMainMenu).ClickAsync();
            await _page.Locator("//a[@href='admin.php?page=mycred-woocommerce']").ClickAsync();
            await _page.Locator("#ui-id-13").ClickAsync();

            var checkBox = await _page.Locator("//input[@id='mycredprefwoomwppartialpaymentsenable']").IsCheckedAsync();

            if (!checkBox)
            {
                await _page.Locator("//input[@id='mycredprefwoomwppartialpaymentsenable']").ClickAsync();
            }

            await _page.Locator("//span[@id='select2-mycredprefwoomwppartialpaymentschangeposition-container']").ClickAsync();

            await _page.WaitForSelectorAsync("ul[id='select2-mycredprefwoomwppartialpaymentschangeposition-results'] li");
            var templateToDisp = await _page.Locator("ul[id='select2-mycredprefwoomwppartialpaymentschangeposition-results'] li").AllAsync();

            foreach (var template in templateToDisp)
            {

                var getText = await template.TextContentAsync();
                if (getText.Contains("In Cart"))
                {
                    await template.ClickAsync();
                    break;
                }

            }

            _page = await _browser.NewPageAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.GotoAsync("http://temp.local/my-account/");
            await _page.Locator("//input[@id='username']").WaitForAsync();
            await _page.Locator("//input[@id='username']").FillAsync("test.cred01@gmail.com");
            await _page.Locator("//input[@id='password']").WaitForAsync();
            await _page.Locator("//input[@id='password']").FillAsync("faraz0096");
            await _page.Locator("//button[normalize-space()='Log in']").ClickAsync();
            await _page.Locator("(//a[normalize-space()='Shop'])[1]").ClickAsync();
            await _page.Locator("(//a[@aria-label='Add to cart: “Album”'])[1]").WaitForAsync();
            await _page.Locator("(//a[@aria-label='Add to cart: “Album”'])[1]").ClickAsync();
            await Task.Delay(2000);
            await _page.Locator("(//a[normalize-space()='Cart'])[1]").WaitForAsync();
            await _page.Locator("(//a[normalize-space()='Cart'])[1]").ClickAsync();

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.Locator("select[id='point-type-select']").SelectOptionAsync("mycred_default");

        }
    }
}
