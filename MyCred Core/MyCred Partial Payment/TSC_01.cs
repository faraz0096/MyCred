using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.MyCred_Partial_Payment
{
    public class TSC_01 : BaseClass
    {
        private string PartialPaymentTitle;


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

            var getPartialPaymentTitle = await _page.Locator("input[id='mycredprefwoomwppartialpaymentstitle']").GetAttributeAsync("value");
            var removeSpacs = getPartialPaymentTitle.Trim();
            Console.WriteLine(removeSpacs);

            await _page.Locator("input[value='Update']").ClickAsync();

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
            //Test will Fail if not Point type dropdown not showing.

            // Check if the Point Type exists on the Checkout page
            var pointTypeDropDown = _page.Locator("select[id='point-type-select']");
            var count = await pointTypeDropDown.CountAsync();

            if (count > 0)
            {
                // If the point-type exists in cart page, interact with it
                Console.WriteLine("Point type drop down found on cart page");
                await pointTypeDropDown.SelectOptionAsync("mycred_default");
                Assert.Pass();
            }
            else
            {
                // If the point-type does not exist, log and fail the test
                Console.WriteLine("Point type dropdown not found on cart page");
                Assert.Fail();
            }
            //Verify the Partial Payment Title showing in cart page or not

            var getTextPartialTitle = await _page.Locator("div[id='mycred-partial-payment-woo'] h3").TextContentAsync();
            var removeSpaced = getTextPartialTitle.Trim();
            PartialPaymentTitle = removeSpaced;

            Assert.That(getPartialPaymentTitle, Is.EqualTo(removeSpaced));
        }

        [Test, Order(2)]
        public async Task PointTypeDropDownCheckout()
        {
            //Verify the point type drop down on checkout page when select Template "In Cart"
            // Point Type drop down should not display on Checkout Page.

            await _page.Locator("//span[@class='wc-block-components-button__text']").ClickAsync();

            // Check if the Point Type exists on the Checkout page, Fail the test case
            var pointTypeDropDown = _page.Locator("select[id='point-type-select']");
            var count = await pointTypeDropDown.CountAsync();

            if (count > 0)
            {
                // If the locator exists, interact with it
                Console.WriteLine("Point type drop down found on checkout page");
                await pointTypeDropDown.SelectOptionAsync("mycred_default");
                Assert.Fail();
            }
            else
            {
                // If the locator does not exist, log and pass the test
                Console.WriteLine("Point type dropdown not found on checkout page");
                Assert.Pass();
            }
        }
        [Test, Order(3)]
        public async Task PointTypeDropDownLegacyCart()
        {
            //Verify the point type drop down on legacy cart page when select Template "In Cart"
            await _page.Locator("(//a[normalize-space()='Legacy cart page'])[1]").ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            //Test will Fail if not Point type dropdown not showing.

            // Check if the Point Type exists on the legacy cart page
            var pointTypeDropDown = _page.Locator("select[id='point-type-select']");
            var count = await pointTypeDropDown.CountAsync();

            if (count > 0)
            {
                // If point-type exists on legacy cart page, interact with it and pass the test
                Console.WriteLine("Point type drop down found on cart page");
                await pointTypeDropDown.SelectOptionAsync("mycred_default");
                Assert.Pass();
            }
            else
            {
                // If the point-type does not exist, log and fail the test
                Console.WriteLine("Point type dropdown not found on cart page");
                Assert.Fail();
            }
            //Verify the Partial Payment Title showing in cart page or not

            var getTextPartialTitle = await _page.Locator("div[id='mycred-partial-payment-woo'] h3").TextContentAsync();
            var removeSpaced = getTextPartialTitle.Trim();

            Assert.That(PartialPaymentTitle, Is.EqualTo(removeSpaced));
        }
    }
}
