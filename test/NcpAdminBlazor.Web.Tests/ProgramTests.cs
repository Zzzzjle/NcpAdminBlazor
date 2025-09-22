using NcpAdminBlazor.Web.Tests.Fixtures;

namespace NcpAdminBlazor.Web.Tests
{
    [Collection(WebAppTestCollection.Name)]
    public class ProgramTests(WebAppFixture app) : TestBase<WebAppFixture>
    {
        [Fact]
        public async Task HealthCheckTestAsync()
        {
            var response = await app.DefaultClient.GetAsync("/health", TestContext.Current.CancellationToken);
            response.IsSuccessStatusCode.ShouldBeTrue();
        }
    }
}