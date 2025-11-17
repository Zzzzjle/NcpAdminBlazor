namespace NcpAdminBlazor.ApiService.Tests.Fixtures;

[CollectionDefinition(Name)]
public class AuthenticatedTestCollection : TestCollection<AuthenticatedAppFixture>
{
    public const string Name = nameof(AuthenticatedTestCollection);
}