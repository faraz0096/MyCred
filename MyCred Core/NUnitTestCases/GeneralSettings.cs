using Microsoft.Playwright;
using MyCred_Core.Base_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCred_Core.NUnitTestCases
{
    public class GeneralSettings : BaseClass
    {
        [Test, Order(1)]
        public async Task GeneralSettingsCapture()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.Locator(MyCredMainMenu).ClickAsync();
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = @"D:\MyCredScreenshots\generalsettingspage.png" });

        }
        [Test, Order(2)]
        public async Task PointTypes()
        {
            await _page.Locator("div[class='mycred-ui-accordion-header ui-accordion-header ui-corner-top ui-state-default ui-accordion-icons ui-accordion-header-collapsed ui-corner-all']").ClickAsync();
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = @"D:\MyCredScreenshots\pointypes.png" });

        }
    }
}
