using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCred_Core.MyCred_Partial_Payment
{
    public class Partial_Payment : BaseClass
    {
        private double exchangeRate = 0;
        private int min = 0;
        private int max = 0;
        private double MaxCouponPercentage = 0;
        private double actualAmountProduct = 0;


        [Test, Order(1)]
        public async Task Settings()
        {
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

            var templateToDisp = await _page.Locator("ul[id='select2-mycredprefwoomwppartialpaymentschangeposition-results'] li").AllAsync();

            foreach(var template in templateToDisp)
            {

                var getText = await template.TextContentAsync();
                if(getText.Contains("Both Cart and Checkout"))
                {
                    await template.ClickAsync();
                }
               
            }

           
            await _page.Locator("#select2-mycredprefwoomwppartialpaymentsmultiple-container").ClickAsync();

            var multiplePayments = await _page.Locator("ul[id='select2-mycredprefwoomwppartialpaymentsmultiple-results'] li").AllAsync();

            foreach (var multi in multiplePayments)
            {

                var getText = await multi.TextContentAsync();
                if (getText.Contains("Yes"))
                {
                    await multi.ClickAsync();
                }

            }

            await _page.Locator("input[value='Update']").ClickAsync();
        }


        [Test, Order(2)]
        public async Task PointsSettings()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.Locator(Points).ClickAsync();
            await _page.Locator("//a[@href='admin.php?page=mycred-settings']").ClickAsync();
            await _page.Locator("div[id='ui-id-7']").ClickAsync();
            await _page.Locator("#generalexchangerate").ClearAsync();
            await _page.Locator("#generalexchangerate").FillAsync("1");
            await _page.Locator("#generalmwpmin").ClearAsync();
            await _page.Locator("#generalmwpmin").FillAsync("1");
            await _page.Locator("#generalmwpmax").ClearAsync();
            await _page.Locator("#generalmwpmax").FillAsync("100");

            await _page.Locator("//input[@id='submit']").ClickAsync();

            await _page.Locator("//a[@href='admin.php?page=mycred-settings']").ClickAsync();
            var getExchangeRateText = await _page.Locator("#generalexchangerate").GetAttributeAsync("value");


            var exchangeRateRemove = getExchangeRateText?.Trim();

            var actualExchangeRate = double.Parse(getExchangeRateText);

            //Reassign to Global Variable
            exchangeRate = actualExchangeRate;

            var getMinValue = await _page.Locator("#generalmwpmin").GetAttributeAsync("value");
            var getMinRemove = getMinValue?.Trim();
            var actualMin = int.Parse(getMinRemove);

            //Reassign to Global Variable
            min = actualMin;

            var getMaxValue = await _page.Locator("#generalmwpmax").GetAttributeAsync("value");
            var getMaxValueRemove = getMaxValue?.Trim();
            var actualMax = int.Parse(getMaxValueRemove);

            //Reassign to Global Variable
            max = actualMax;

            Console.WriteLine("Exchange Rate: " + exchangeRate);
            Console.WriteLine("Minimum Value: " + min);
            Console.WriteLine("Minimum Value: " + max);
            await _page.CloseAsync();

        }

        [Test, Order(3)]
        public async Task CartPagePartialPayment()
        {
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
            var partialPayment = await _page.Locator("div[id='mycred-partial-payment-woo'] h3").TextContentAsync();
          Console.WriteLine(partialPayment);

            await _page.Locator("select[id='point-type-select']").SelectOptionAsync("mycred_default");
            //Get cart sub-totals
            var getTotalAmount = await _page.Locator(".wc-block-formatted-money-amount.wc-block-components-formatted-money-amount.wc-block-components-totals-item__value").TextContentAsync();
            var removeSymbols = getTotalAmount.Substring(1);
            var actualAmount = double.Parse(removeSymbols);
            //Re-Aassign to Global var
            actualAmountProduct = actualAmount;
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Get point type min and max 
            await _page.WaitForSelectorAsync("div[id='mycred-partial-payment-total'] h4");
            await Task.Delay(3000);
            var maximumPercentage = await _page.Locator("div[id='mycred-partial-payment-total'] h4").TextContentAsync();
            var parts = maximumPercentage.Split(" ");
            var minAmount = parts[4]; // "1.00"
            var maxAmount = parts[8]; // "20.00"

            var convertMin = double.Parse(minAmount);
            var convertMax = double.Parse(maxAmount);
            Console.WriteLine($"Minimum: {convertMin}, Maximum: {convertMax}");

            //Verify the min point type is equal to minimum which is apply from backend
            Assert.That(convertMin, Is.EqualTo(min), "Minimum is incorrect");

            var calcMaxPercentage = (actualAmount * max) / 100;

            MaxCouponPercentage = calcMaxPercentage;

            //Verify the max % point type is equal to maximum which is apply from backend
            Assert.That(calcMaxPercentage, Is.EqualTo(convertMax), "Maximum is incorrect");

            var getUserBalance = await _page.Locator("div[class='wc-block-components-order-meta css-0 e19lxcc00'] div:nth-child(1) div:nth-child(2) span:nth-child(2)").TextContentAsync();
            var convertUserBalance = double.Parse(getUserBalance);

            //Apply full coupon discount if user balance is greater than or equal to sub-total
            if (convertUserBalance >= actualAmount && actualAmount == calcMaxPercentage)
            {

                await _page.Locator("input[class='mycred-partial-payment-inpt']").ClearAsync();
                
                await _page.Locator("input[class='mycred-partial-payment-inpt']").FillAsync(removeSymbols);
                //click out side input to visible Apply Discount button

                await _page.Locator("div[class='wc-block-components-order-meta css-0 e19lxcc00'] div:nth-child(1) div:nth-child(2) span:nth-child(2)").ClickAsync();
                await _page.Locator("input[value='Apply Discount']").WaitForAsync();

                await _page.Locator("input[value='Apply Discount']").ClickAsync();

                var getCoupons = await _page.Locator("div[class='wc-block-components-totals-item wc-block-components-totals-discount'] span[class='wc-block-formatted-money-amount wc-block-components-formatted-money-amount wc-block-components-totals-item__value']").TextContentAsync();
               
                var regex = new Regex(@"\d+(\.\d+)?");
                var match = regex.Match(getCoupons);
                var coupon = match.ToString();
                var convertCoupons = double.Parse(coupon);

                Assert.That(convertCoupons, Is.EqualTo(actualAmount));

                var totalAmount = await _page.Locator(".wc-block-formatted-money-amount.wc-block-components-formatted-money-amount.wc-block-components-totals-footer-item-tax-value").TextContentAsync();
                var matchTotal = regex.Match(totalAmount);
                var totalAmountBefore = matchTotal.ToString();
                var convertTotalAmount = double.Parse(totalAmountBefore);

                if(convertTotalAmount == 0)
                {
                    Console.WriteLine("Coupon applied successfully");
                }

               
            }
            else
            {
                Console.WriteLine("User Balance is less: " + convertUserBalance);
            }

        }


        [Test, Order(4)]
        public async Task RemoveCoupon()
        {
            //Verify the error message when enter amount greater than maximum
            await _page.Locator("button[class='wc-block-components-chip__remove']").ClickAsync();
            await _page.ReloadAsync();
            await Task.Delay(2000);
       
            double incrementMaxCoupon = MaxCouponPercentage;
            incrementMaxCoupon++;
            string convertString = incrementMaxCoupon.ToString();
            await _page.WaitForSelectorAsync("select[id='point-type-select']");
            await _page.Locator("select[id='point-type-select']").SelectOptionAsync("mycred_default");
            await _page.Locator("input[class='mycred-partial-payment-inpt']").ClearAsync();

            await _page.Locator("input[class='mycred-partial-payment-inpt']").FillAsync(convertString);
            await _page.Locator("div[class='wc-block-components-order-meta css-0 e19lxcc00'] div:nth-child(1) div:nth-child(2) span:nth-child(2)").ClickAsync();
            await _page.Locator("input[value='Apply Discount']").WaitForAsync();

            await _page.Locator("input[value='Apply Discount']").ClickAsync();

            var getError =  await _page.Locator("//div[@class='wc-block-components-notice-banner__content']//div[contains(text(),'The amount can not be greater than the maximum amo')]").TextContentAsync();

            Assert.That(getError, Is.EqualTo("The amount can not be greater than the maximum amount."));

            Console.WriteLine(getError);
            await Task.Delay(2000);

        }

        [Test, Order(4)]

        public async Task MultipleCoupons()
        {
            await _page.ReloadAsync();
            await _page.Locator("select[id='point-type-select']").SelectOptionAsync("mycred_default");
            await _page.Locator("input[class='mycred-partial-payment-inpt']").ClearAsync();
            double actualMulti = actualAmountProduct;
            //Half to actual amount apply for multiple coupons

            double change = (actualMulti * 50) / 100;
            var changeToString = change.ToString();
            await Task.Delay(3000);
            var maximumPercentage = await _page.Locator("div[id='mycred-partial-payment-total'] h4").TextContentAsync();
            var parts = maximumPercentage.Split(" ");
            var minAmount = parts[4]; // "1.00"
            var maxAmount = parts[8]; // "20.00"

            var convertMin = double.Parse(minAmount);
            var convertMax = double.Parse(maxAmount);
            var calcMaxPercentage = (actualMulti * max) / 100;

            Console.WriteLine("Initial Maximum percentage: " + calcMaxPercentage);

            await _page.Locator("input[class='mycred-partial-payment-inpt']").FillAsync(changeToString);
            await _page.Locator("div[class='wc-block-components-order-meta css-0 e19lxcc00'] div:nth-child(1) div:nth-child(2) span:nth-child(2)").ClickAsync();
            await _page.Locator("input[value='Apply Discount']").WaitForAsync();

            await _page.Locator("input[value='Apply Discount']").ClickAsync();
            await Task.Delay(8000);
            await _page.WaitForSelectorAsync("select[id='point-type-select']");
            await _page.Locator("select[id='point-type-select']").SelectOptionAsync("mycred_default");
            await _page.WaitForSelectorAsync("div[id='mycred-partial-payment-total'] h4");
            var maximumPercentageMulti = await _page.Locator("div[id='mycred-partial-payment-total'] h4").TextContentAsync();
            var partsMulti = maximumPercentageMulti.Split(" ");
            var minAmountMulti = partsMulti[4]; // "1.00"
            var maxAmountMulti = partsMulti[8]; // "20.00"
            var convertMinMulti = double.Parse(minAmountMulti);
            var convertMaxMulti = double.Parse(maxAmountMulti);
            Console.WriteLine("Remaining Maximum percentage: " + convertMaxMulti);

            string sendMaxToInput = convertMaxMulti.ToString();
            await _page.Locator("input[class='mycred-partial-payment-inpt']").ClearAsync();
            await _page.Locator("input[class='mycred-partial-payment-inpt']").FillAsync(sendMaxToInput);
            await _page.Locator("div[class='wc-block-components-order-meta css-0 e19lxcc00'] div:nth-child(1) div:nth-child(2) span:nth-child(2)").ClickAsync();
            await _page.Locator("input[value='Apply Discount']").WaitForAsync();

            await Task.Delay(1000);
            await _page.Locator("input[value='Apply Discount']").ClickAsync();
            var regex = new Regex(@"\d+(\.\d+)?");
            var totalAmount = await _page.Locator(".wc-block-formatted-money-amount.wc-block-components-formatted-money-amount.wc-block-components-totals-footer-item-tax-value").TextContentAsync();
            var matchTotal = regex.Match(totalAmount);
            var totalAmountBefore = matchTotal.ToString();
            var convertTotalAmount = double.Parse(totalAmountBefore);

            if (convertTotalAmount == 0)
            {
                Console.WriteLine("Multiple Coupons applied successfully");
            }


        }
    }
}
