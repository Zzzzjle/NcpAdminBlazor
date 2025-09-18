namespace NcpAdminBlazor.Client.Models
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
