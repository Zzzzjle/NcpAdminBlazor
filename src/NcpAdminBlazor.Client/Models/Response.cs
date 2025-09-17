namespace NcpAdminBlazor.Client.Client.Models
{
    public class Response
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Error { get; set; } = [];
        public string Token { get; set; } = string.Empty;
    }
}
