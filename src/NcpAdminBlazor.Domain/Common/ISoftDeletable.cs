namespace NcpAdminBlazor.Domain.Common;

public interface ISoftDeletable
{
    public Deleted IsDeleted { get; }
    public DeletedTime DeletedAt { get; }
    public void Delete();
}