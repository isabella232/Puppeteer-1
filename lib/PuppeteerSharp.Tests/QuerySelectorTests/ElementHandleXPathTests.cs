using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleXPathTests : PuppeteerPageBaseTest
    {
        public ElementHandleXPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$x", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await DevToolsContext.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">A</div></div></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var second = await html.XPathAsync("./body/div[contains(@class, 'second')]");
            var inner = await second[0].XPathAsync("./div[contains(@class, 'inner')]");
            var content = await DevToolsContext.EvaluateFunctionAsync<string>("e => e.textContent", inner[0]);
            Assert.Equal("A", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$x", "should return null for non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await DevToolsContext.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">B</div></div></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var second = await html.XPathAsync("/div[contains(@class, 'third')]");
            Assert.Empty(second);
        }
    }
}
