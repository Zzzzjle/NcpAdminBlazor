namespace NcpAdminBlazor.Web.Tests.Fixtures;

public sealed class UserTestState : StateFixture
{
    public string TestUserName { get; set; } = string.Empty;
    public string TestUserEmail { get; private set; } = string.Empty;
    public string TestUserPassword { get; set; } = string.Empty;
    public string TestUserPhone { get; set; } = string.Empty;
    public string TestUserRealName { get; set; } = string.Empty;
    public string? TestUserId { get; set; }
    public string? AuthToken { get; set; }

    protected override ValueTask SetupAsync()
    {
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        TestUserName = $"testuser{uniqueId}";
        TestUserEmail = $"test{uniqueId}@example.com";
        TestUserPassword = "TestPassword123";
        TestUserPhone = "13800138000";
        TestUserRealName = "测试用户";
        
        return ValueTask.CompletedTask;
    }

    protected override ValueTask TearDownAsync()
    {
        TestUserId = null;
        AuthToken = null;
        return ValueTask.CompletedTask;
    }
}