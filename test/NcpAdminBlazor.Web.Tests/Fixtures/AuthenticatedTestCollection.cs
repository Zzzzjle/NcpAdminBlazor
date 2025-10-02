namespace NcpAdminBlazor.Web.Tests.Fixtures;

[CollectionDefinition(Name)]
public class AuthenticatedTestCollection : TestCollection<AuthenticatedAppFixture>
{
    public const string Name = nameof(AuthenticatedTestCollection);
}