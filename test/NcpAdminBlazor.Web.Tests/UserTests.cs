// using System.Net;
// using System.Net.Http.Json;
// using NcpAdminBlazor.Infrastructure;
// using NcpAdminBlazor.Web.Endpoints.UserEndpoints;
// using NcpAdminBlazor.Shared.Models;
// using Microsoft.EntityFrameworkCore;
// using NetCorePal.Extensions.Dto;
//
// namespace NcpAdminBlazor.Web.Tests;
//
// public class UserTests : IClassFixture<MyWebApplicationFactory>
// {
//     private readonly MyWebApplicationFactory _factory;
//     private readonly HttpClient _client;
//
//     public UserTests(MyWebApplicationFactory factory)
//     {
//         using (var scope = factory.Services.CreateScope())
//         {
//             var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//             db.Database.Migrate();
//         }
//
//         _factory = factory;
//         _client = factory.WithWebHostBuilder(builder => { builder.ConfigureServices(p => { }); }).CreateClient();
//     }
//
//     [Fact]
//     public async Task Login_And_Auth_Test()
//     {
//         string userName = "testname";
//         string password = "testpassword";
//         var loginRequest = new LoginRequest 
//         { 
//             LoginName = userName, 
//             Password = password 
//         };
//         var response = await _client.PostAsJsonAsync($"/api/user/login", loginRequest);
//         response.IsSuccessStatusCode.ShouldBeTrue();
//         var responseData = await response.Content.ReadFromNewtonsoftJsonAsync<ResponseData<string>>();
//         responseData.ShouldNotBeNull();
//         responseData.Data.ShouldNotBeNull();
//
//         var jwtResponse1 = await _client.GetAsync("/api/user/auth");
//         jwtResponse1.IsSuccessStatusCode.ShouldBeFalse();
//         jwtResponse1.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
//
//         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseData.Data);
//         var jwtResponse2 = await _client.GetAsync("/api/user/auth");
//         jwtResponse2.IsSuccessStatusCode.ShouldBeTrue();
//     }
// }