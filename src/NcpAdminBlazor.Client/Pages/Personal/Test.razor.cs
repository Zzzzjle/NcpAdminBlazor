using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Client.Pages.Personal
{
    public class TestBase : ComponentBase
    {
        protected List<Student> Students = new List<Student>();


        [Inject]
        public HttpClient HttpClient { get; set; } = null!;
        protected async Task GetRecords()
        {
            Students = await HttpClient.GetFromJsonAsync<List<Student>>("/api/Students") ?? [];
        }
    }
}
