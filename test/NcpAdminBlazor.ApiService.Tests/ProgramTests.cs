using NcpAdminBlazor.ApiService.Tests.Fixtures;

namespace NcpAdminBlazor.ApiService.Tests
{
    [Collection(WebAppTestCollection.Name)]
    public class ProgramTests(WebAppFixture app) : TestBase<WebAppFixture>
    {
        [Fact]
        public async Task HealthCheckTestAsync()
        {
            var response = await app.Client.GetAsync("/health", TestContext.Current.CancellationToken);
            response.IsSuccessStatusCode.ShouldBeTrue();
        }
    }
}