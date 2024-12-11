using Microsoft.Playwright;
using MyCred_Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.Base_Test
{
    public class BaseClass
    {
        protected IPlaywright _playwright;
        protected IBrowser _browser;
        protected IPage _page;

        protected readonly ConfigReader _config = new ConfigReader();

        protected string BaseUrl => _config.GetConfigValue("BaseUrl");
        protected string Username => _config.GetConfigValue("Username");
        protected string Password => _config.GetConfigValue("Password");
        protected string LoginBtn => _config.GetConfigValue("LoginBtn");
        protected string MyCredMainMenu => _config.GetConfigValue("MyCredMenu");
        protected string Update => _config.GetConfigValue("UpdateSettings");
        protected string Points => _config.GetConfigValue("PointType");
        protected string hooks => _config.GetConfigValue("HooksMenu");
        protected string Addons => _config.GetConfigValue("AddonsMenu");
        protected string ActDeacAddons => _config.GetConfigValue("ActivateDeactivateAddons");
        protected string LeftHooksWidget => _config.GetConfigValue("LefSideHooksWidget");
        protected string ActionBtnPointsLoggin => _config.GetConfigValue("ActBtnPntLogin");
        protected string PointsLogginInput => _config.GetConfigValue("PntsForLogginInput");
        protected string SaveBtnLoggingHook => _config.GetConfigValue("SaveBtnPntsLogging");
        protected string RightSideWidget => _config.GetConfigValue("RightSideHooksWidget");
        protected string LimitLoggin => _config.GetConfigValue("HooksLogginLimit");
        protected string PntsRewardNotif => _config.GetConfigValue("NotificationPointsReward");
        protected string GetUserPnts => _config.GetConfigValue("GetWordPressUserPoints");

        [OneTimeSetUp]

        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Timeout = 80000
            });

            _page = await _browser.NewPageAsync();
            await _page.SetViewportSizeAsync(1920, 1080);

            await _page.GotoAsync(BaseUrl);

            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            await _page.FillAsync("#user_login", Username);
            await _page.FillAsync("#user_pass", Password);
            await _page.ClickAsync($"id={LoginBtn}");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
           
        }
    }
}

