using System.Net;
using System.Text.Json;
using TaskBoard.Api.Data;
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

    private async Task<TaskItem> CreateTaskAsync(string title)
    {
        var newTask = new StringContent($"{{\"Title\":\"{title}\"}}", System.Text.Encoding.UTF8, "application/json");
        var postResponse = await _client.PostAsync("/tasks", newTask);
        postResponse.EnsureSuccessStatusCode();

        var body = await postResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<TaskItem>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (created == null)
        {
            throw new InvalidOperationException("POST /tasks did not return a task payload.");
        }

        return created;
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
    }

    [Fact]
    public async Task GET_Task_By_ID_Bad_ID()
    {
        var created = await CreateTaskAsync("Seed Task");
        var response = await _client.GetAsync($"/tasks/{created.Id + 1}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Task_By_ID()
    {
        var created = await CreateTaskAsync("New Task");
        var response = await _client.GetAsync($"/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("New Task", body);
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
        var newTask = new StringContent("{\"Title\":\"New Task\"}", System.Text.Encoding.UTF8, "application/json");
        var postResponse = await _client.PostAsync("/tasks", newTask);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var getResponse = await _client.GetAsync("/tasks");
        var body = await getResponse.Content.ReadAsStringAsync();
        Assert.Contains("New Task", body);
    }

    [Fact]
    public async Task POST_Empty_Title_Returns_BadRequest()
    {
        var newTask = new StringContent("{\"Title\":\"\"}", System.Text.Encoding.UTF8, "application/json");
        var postResponse = await _client.PostAsync("/tasks", newTask);
        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
    }

    [Fact]
    public async Task PUT_Empty_Title_Returns_BadRequest()
    {
        var created = await CreateTaskAsync("New Task");

        var updatedTask = new StringContent("{\"Title\":\"\"}", System.Text.Encoding.UTF8, "application/json");
        var putResponse = await _client.PutAsync($"/tasks/{created.Id}", updatedTask);
        Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
    }

    [Fact]
    public async Task PUT_Bad_ID_Returns_NotFound()
    {
        var updatedTask = new StringContent("{\"Title\":\"Updated Task\"}", System.Text.Encoding.UTF8, "application/json");
        var putResponse = await _client.PutAsync($"/tasks/9999", updatedTask);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }

    [Fact]
    public async Task PUT_Task()
    {
        var created = await CreateTaskAsync("New Task");

        var updatedTask = new StringContent("{\"Title\":\"Updated Task\"}", System.Text.Encoding.UTF8, "application/json");
        var putResponse = await _client.PutAsync($"/tasks/{created.Id}", updatedTask);
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/tasks/{created.Id}");
        var body = await getResponse.Content.ReadAsStringAsync();
        Assert.Contains("Updated Task", body);
    }

    [Fact]
    public async Task DELETE_Task()
    {
        var created = await CreateTaskAsync("New Task");
        var deleteResponse = await _client.DeleteAsync($"/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
