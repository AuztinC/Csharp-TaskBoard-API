using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace TaskBoard.Tests;

public class PingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public PingTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task GET_Ping_Returns_Pong()
    {
        var response = await _client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);   

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("pong", body);

    }
}
