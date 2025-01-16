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
    public class PointsForDailyVisit : BaseClass
    {
        private const string PointsForDailyVisits = "12";
        private const string DailyVisitHookUser = "pntsdaily01@gmail.com";
        private const string UserName = "points daily";
        private const string HookReference = "Website Visit";
        private const string DateTimeFormat = "MMMM dd, yyyy h:mm tt";
        private const string Entry = "Points for site visit";
        private double PointsBeforeReward = 0;
        private string CapturedDateTime;

        protected string PointTypes => _config.GetConfigValue("PointTypesAccordion");


        [Test, Order(1)]
        // [Ignore("test")]
        public async Task PointsForDailyVisitHook()
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
                if (hooksText.Contains("Points for daily visits"))
                {
                    Console.WriteLine("Found 'Points for daily visits' in the left-side panel.");

                    // Click the action button in the left panel
                    await _page.Locator(ActionBtnPntsDailyVisit).ClickAsync();
                    isHookFound = true;
                    await _page.Locator(PointsDailyInput).FillAsync(PointsForDailyVisits);

                    //Save Hook
                    await _page.Locator(SaveBtnDailyVisit).ClickAsync();
                    break; // Exit the loop after handling the hook
                }
            }
            // If not found in the left-side panel, check the right-side panel
            if (!isHookFound)
            {
                Console.WriteLine("'Points for daily visit' not found in the left-side panel. Checking the right-side panel...");

                // Get all hooks in the right-side panel
                var activeHooks = await _page.Locator(RightSideWidget).AllAsync();

                foreach (var active in activeHooks)
                {
                    var activeHookText = await active.TextContentAsync();

                    // Check if the hook matches "Points for logins"
                    if (activeHookText.Contains("Points for daily visits"))
                    {
                        Console.WriteLine("Found 'Points for daily visit' in the right-side panel.");

                        // Click the hook in the right panel
                        await active.ClickAsync();
                        await _page.Locator(PointsDailyInput).FillAsync(PointsForDailyVisits);
                        await _page.Locator("//div[@id='sidebar-active']//input[@id='widget-mycred-hook-site_visit-__i__-savewidget']").ClickAsync();
                        break; // Exit the loop after handling the hook
                    }
                }
            }

        }

        [Test, Order(3)]
        public async Task CheckUserPoints()
        {
            await _page.Locator("//div[normalize-space()='Users']").ClickAsync();
            await _page.Locator("//input[@id='user-search-input']").FillAsync(DailyVisitHookUser);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator("//td[@class='mycred_default column-mycred_default']/div[1]/span[1]").TextContentAsync();
            Console.WriteLine(getUserPoints);
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            Console.WriteLine($"User Actual Points: {removeSymbol}");
            PointsBeforeReward = double.Parse(removeSymbol, CultureInfo.InvariantCulture);
        }

        [Test, Order(4)]
        public async Task LoginUser()
        {
            _page = await _browser.NewPageAsync();
            await _page.GotoAsync("https://wordpress-1077016-4396807.cloudwaysapps.com/my-account/");
            await _page.Locator("input[name='username']").FillAsync(DailyVisitHookUser);
            await _page.Locator("input[name='password']").FillAsync("faraz0096");
            await _page.Locator("button[name='login']").ClickAsync();

            var getNotificationText = await _page.Locator("div[class='notice-item succes'] p").TextContentAsync();

            if (getNotificationText.Contains("Points for site visit"))
            {
                Console.WriteLine(getNotificationText);
                await _page.Locator(PntsRewardNotif).WaitForAsync();

                var getPointText = await _page.Locator(PntsRewardNotif).TextContentAsync();

                Console.WriteLine($"Points Rewarded: {getPointText}");
            }
            //Capture date and time immediately
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
            await _page.Locator("//input[@id='user-search-input']").FillAsync(DailyVisitHookUser);
            await _page.Locator("//input[@id='search-submit']").ClickAsync();

            var getUserPoints = await _page.Locator(GetUserPnts).TextContentAsync();
            var removeSymbol = getUserPoints.Replace("$", "").Trim();
            double actualPoints = double.Parse(removeSymbol, CultureInfo.InvariantCulture);

            double hookPoints = double.Parse(PointsForDailyVisits, CultureInfo.InvariantCulture);
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
            await _page.Locator("//input[@id='myCRED-user-filter']").FillAsync(DailyVisitHookUser);
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
            if (getLogPoints.Contains(PointsForDailyVisits))
            {
                Console.WriteLine("Points are correct: " + getLogPoints);
            }

            // Assert.That(getLogPoints, Is.EqualTo(PointsForLogging), "Incorrect points or points not rewarded");


            var getEntry = await _page.Locator("td[class='column-entry']").First.TextContentAsync();
            if (getEntry.Contains(Entry))
            {
                Console.WriteLine("Hook Entry is correct: " + getEntry);
            }
            Assert.That(getEntry, Is.EqualTo(Entry), "Entry Log is incorrect");
        }
    }
}
