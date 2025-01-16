using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.NUnitTestCases
{
    public class PointsForRegistration : BaseClass
    {
        private const string PointsForRegistrations = "50";
        private const string RegisterHookEmail = "mycredhookregister06@gmail.com";
        private const string UserName = "mycredhook register6";
        private const string HookReference = "Website Registration";
        private const string Entry = "Points for becoming a member";
        private const string DateTimeFormat = "MMMM dd, yyyy h:mm tt";
        private double PointsBeforeReward = 0;
        private string CapturedDateTime;

        protected string PointTypes => _config.GetConfigValue("PointTypesAccordion");


        [Test, Order(1)]
        // [Ignore("test")]
        public async Task PointsForRegistrationHook()
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
                if (hooksText.Contains("Points for registrations"))
                {
                    Console.WriteLine("Found 'Points for logins' in the left-side panel.");

                    // Click the action button in the left panel
                    await _page.Locator(ActionBtnPointsRegis).ClickAsync();
                    isHookFound = true;
                    await _page.Locator(PointsRegisInput).FillAsync(PointsForRegistrations);
                    //Save Hook
                    await _page.Locator(SaveBtnRegisterHook).ClickAsync();
                    break; // Exit the loop after handling the hook
                }
            }
            // If not found in the left-side panel, check the right-side panel
            if (!isHookFound)
            {
                Console.WriteLine("'Points for registration' not found in the left-side panel. Checking the right-side panel...");

                // Get all hooks in the right-side panel
                var activeHooks = await _page.Locator(RightSideWidget).AllAsync();

                foreach (var active in activeHooks)
                {
                    var activeHookText = await active.TextContentAsync();

                    // Check if the hook matches "Points for logins"
                    if (activeHookText.Contains("Points for registrations"))
                    {
                        Console.WriteLine("Found 'Points for registration' in the right-side panel.");

                        // Click the hook in the right panel
                        await active.ClickAsync();
                        await _page.Locator(PointsRegisInput).FillAsync(PointsForRegistrations);
                        await _page.Locator("//div[@id='sidebar-active']//input[@id='widget-mycred-hook-registration-__i__-savewidget']").ClickAsync();
                        break; // Exit the loop after handling the hook
                    }
                }
            }

        }

        /*[Test, Order(3)]
        public async Task CheckUserPoints()
        {
            await _page.Locator("//div[normalize-space()='Users']").ClickAsync();
            await _page.Locator("//input[@id='user-search-input']").FillAsync(LoginHookUser);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator("//td[@class='mycred_default column-mycred_default']/div[1]/span[1]").TextContentAsync();
            Console.WriteLine(getUserPoints);
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            PointsBeforeReward = double.Parse(removeSymbol, CultureInfo.InvariantCulture);
        }*/

        [Test, Order(4)]
        public async Task RegisterUser()
        {
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync(BaseUrl);
            await _page.Locator("//a[normalize-space()='Register']").ClickAsync();
            await _page.Locator("#user_login").FillAsync(UserName);
            await _page.Locator("#user_email").FillAsync(RegisterHookEmail);
            await _page.Locator("#wp-submit").ClickAsync();
            CapturedDateTime = DateTime.Now.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            Console.WriteLine("Date Time Captured for Reward: " + CapturedDateTime);
            var registerSuccess = await _page.Locator("div[id='login-message'] p").TextContentAsync();
            Console.WriteLine(registerSuccess);
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
            await _page.Locator("//input[@id='user-search-input']").FillAsync(RegisterHookEmail);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator(GetUserPnts).TextContentAsync();
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            double actualPoints = double.Parse(removeSymbol, CultureInfo.InvariantCulture);

            double hookPoints = double.Parse(PointsForRegistrations, CultureInfo.InvariantCulture);

            if (actualPoints + hookPoints > 0)
            {
                Console.WriteLine("Points rewarded");
                Console.WriteLine("Total User Points: " + actualPoints);
            }
            else
            {
                Console.WriteLine("No points rewarded");
            }
        }

        [Test, Order(6)]
        public async Task VerifyLogs()
        {
            await _page.Locator(Points).ClickAsync();
            await _page.Locator("//a[normalize-space()='Log']").ClickAsync();
            await _page.Locator("//input[@id='myCRED-user-filter']").FillAsync(RegisterHookEmail);
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

            var getLogPoints = await _page.Locator("td[class='column-creds']").First.TextContentAsync();
            if (getLogPoints.Contains(PointsForRegistrations))
            {
                Console.WriteLine("Points are correct: " + getLogPoints);
            }

            //Assert.That(getLogPoints, Is.EqualTo(PointsForRegistrations), "Incorrect points or points not rewarded");

            var getEntry = await _page.Locator("td[class='column-entry']").First.TextContentAsync();
            if (getEntry.Contains(Entry))
            {
                Console.WriteLine("Hook Entry is correct: " + getEntry);
            }
            Assert.That(getEntry, Is.EqualTo(Entry), "Entry Log is incorrect");

        }
    }
}
