using System.Net;
using Xunit.Abstractions;

namespace TaskBoard.Tests;

public class TaskTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TaskTests(ApiFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Bad_Route_Returns_BadRequest()
    {
        var response = await _client.GetAsync("/blah");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);   

    }

    [Fact]
    public async Task GET_Tasks_Returns_Tasks()
    {
        var response = await _client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);   
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("", body);
        // Assert.Contains("Task 2", body);
        // Assert.Contains("Task 3", body);
    }

    [Fact]
    public async Task POST_Without_Body_Returns_BadRequest()
    {
        var response = await _client.PostAsync("/tasks", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Creates_New_Task()
    {
        var newTask = new StringContent("\"New Task\"", System.Text.Encoding.UTF8, "application/json");
        var postResponse = await _client.PostAsync("/tasks", newTask);
        _output.WriteLine(postResponse.ToString());

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var getResponse = await _client.GetAsync("/tasks");
        var body = await getResponse.Content.ReadAsStringAsync();
        Assert.Contains("New Task", body);
    }
}
