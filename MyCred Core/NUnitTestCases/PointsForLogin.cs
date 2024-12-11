using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MyCred_Core.NUnitTestCases
{
    public class PointsForLogin : BaseClass
    {
        private const string PointsForLogging = "10.5";
        private const string LoginHookUser = "mycredhookslogin01@gmail.com";
        private const string UserName = "mycredhook login";
        private const string HookReference = "Logging in";
        private const string DateTimeFormat = "MMMM dd, yyyy h:mm tt";
        private double PointsBeforeReward = 0;
        private string CapturedDateTime;

        protected string PointTypes => _config.GetConfigValue("PointTypesAccordion");

        [Test, Order(1)]
        [Ignore("test")]
        public async Task GeneralSettings()
        {
            // Save general settings
            await _page.Locator(MyCredMainMenu).ClickAsync();
            await _page.Locator(PointTypes).ClickAsync();
            await _page.Locator(Update).ClickAsync();

            // Activate all Addons
            await _page.Locator(Addons).ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var addonsElements = await _page.Locator(ActDeacAddons).AllAsync();
            foreach (var element in addonsElements)
            {
                var textBeforeActivate = await element.TextContentAsync();
                if (textBeforeActivate.Contains("Activate"))
                {
                    await element.ClickAsync();
                }
            }
        }

        [Test, Order(2)]
        // [Ignore("test")]
        public async Task PointsForLoginHook()
        {
            await _page.Locator(Points).ClickAsync();
            await _page.Locator(hooks).ClickAsync();

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            // Get all hooks in the left-side panel
            var hooksAll = await _page.Locator(LeftHooksWidget).AllAsync();
            bool isHookFound = false;

            // Iterate through the left-side panel hooks
            foreach (var hooks in hooksAll)
            {
                var hooksText = await hooks.TextContentAsync();

                // Check if the hook matches "Points for logins"
                if (hooksText.Contains("Points for logins"))
                {
                    Console.WriteLine("Found 'Points for logins' in the left-side panel.");

                    // Click the action button in the left panel
                    await _page.Locator(ActionBtnPointsLoggin).ClickAsync();
                    isHookFound = true;
                    await _page.Locator(PointsLogginInput).FillAsync(PointsForLogging);
                    await _page.Locator(LimitLoggin).FillAsync("50");
                    //Save Hook
                    await _page.Locator(SaveBtnLoggingHook).ClickAsync();
                    break; // Exit the loop after handling the hook
                }
            }
            // If not found in the left-side panel, check the right-side panel
            if (!isHookFound)
            {
                Console.WriteLine("'Points for logins' not found in the left-side panel. Checking the right-side panel...");

                // Get all hooks in the right-side panel
                var activeHooks = await _page.Locator(RightSideWidget).AllAsync();

                foreach (var active in activeHooks)
                {
                    var activeHookText = await active.TextContentAsync();

                    // Check if the hook matches "Points for logins"
                    if (activeHookText.Contains("Points for logins"))
                    {
                        Console.WriteLine("Found 'Points for logins' in the right-side panel.");

                        // Click the hook in the right panel
                        await active.ClickAsync();
                        await _page.Locator(PointsLogginInput).FillAsync(PointsForLogging);
                        await _page.Locator(LimitLoggin).FillAsync("50");
                        await _page.Locator("//div[@id='sidebar-active']//input[@id='widget-mycred-hook-logging_in-__i__-savewidget']").ClickAsync();
                        break; // Exit the loop after handling the hook
                    }
                }
            }

        }

        [Test, Order(3)]
        public async Task CheckUserPoints()
        {
            await _page.Locator("//div[normalize-space()='Users']").ClickAsync();
            await _page.Locator("//input[@id='user-search-input']").FillAsync(LoginHookUser);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator("//td[@class='mycred_default column-mycred_default']/div[1]/span[1]").TextContentAsync();
            Console.WriteLine(getUserPoints);
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            PointsBeforeReward = double.Parse(removeSymbol, CultureInfo.InvariantCulture);
        }

        [Test, Order(4)]
        public async Task LoginUser()
        {
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync("https://wordpress-1077016-4396807.cloudwaysapps.com/my-account/");
            await _page.Locator("input[name='username']").FillAsync(LoginHookUser);
            await _page.Locator("input[name='password']").FillAsync("faraz0096");
            await _page.Locator("button[name='login']").ClickAsync();

            await _page.Locator(PntsRewardNotif).WaitForAsync();

            CapturedDateTime = DateTime.Now.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            Console.WriteLine("Date Time Captured for Reward: " + CapturedDateTime);
        }

        [Test, Order(5)]
        public async Task VerifyPoints()
        {
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync(BaseUrl);
            await _page.FillAsync("#user_login", Username);
            await _page.FillAsync("#user_pass", Password);
            await _page.ClickAsync($"id={LoginBtn}");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await _page.Locator("//div[normalize-space()='Users']").ClickAsync();
            await _page.Locator("//input[@id='user-search-input']").FillAsync(LoginHookUser);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator(GetUserPnts).TextContentAsync();
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            double actualPoints = double.Parse(removeSymbol, CultureInfo.InvariantCulture);

            double hookPoints = double.Parse(PointsForLogging, CultureInfo.InvariantCulture);

            Console.WriteLine(actualPoints >= PointsBeforeReward + hookPoints
                ? "Points rewarded correctly"
                : "Points not rewarded correctly");
        }

        [Test, Order(6)]
        public async Task VerifyLogs()
        {
            await _page.Locator(Points).ClickAsync();
            await _page.Locator("//a[normalize-space()='Log']").ClickAsync();
            await _page.Locator("//input[@id='myCRED-user-filter']").FillAsync(LoginHookUser);
            await _page.Locator("input[value='Filter']").ClickAsync();

            var getUserName = await _page.Locator("td[class='column-primary column-username'] strong").First.TextContentAsync();
            if (getUserName.Contains(UserName))
            {
                Console.WriteLine("Username is correct: " + getUserName);
            }
            Assert.That(getUserName, Is.EqualTo(UserName), "Username is incorrect");

            var getLogRef = await _page.Locator("td[class='column-ref']").First.TextContentAsync();
            if (getLogRef.Contains(HookReference))
            {
                Console.WriteLine("Hook Reference is correct: " + getLogRef);
            }

            Assert.That(getLogRef, Is.EqualTo(HookReference), "Hook Reference is incorrect");

            var getDateTime = await _page.Locator("td[class='column-time'] span").First.TextContentAsync();
            DateTime parsedDateTime = DateTime.ParseExact(getDateTime, DateTimeFormat, CultureInfo.InvariantCulture);

            string formattedDateTime = parsedDateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            if (formattedDateTime.Contains(CapturedDateTime, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Date time is correct: " + getDateTime);
            }
            Assert.That(formattedDateTime, Is.EqualTo(CapturedDateTime), "Date-Time is incorrect");
        }
    }
}
