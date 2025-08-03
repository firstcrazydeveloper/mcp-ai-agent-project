using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Net.Http.Headers;
using System.Text;


namespace MCPAIAgentProject;

public class MCPAgent
{
    private readonly ILogger<MCPAgent> _logger;
  
    private string endpoint;
    private readonly string key;
    private readonly string deploymentName = "gpt-35-turbo";
    string apiVersion = "2025-01-01-preview";  // Use the latest API version
    private readonly string httpApiEndpoint;


    public MCPAgent(ILogger<MCPAgent> logger)
    {
        _logger = logger;
    }

    [Function("MCPAgentAPI")]
    public async Task<IActionResult> MCPAgentAPI([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Fix for 'RequestUri' error: Use req.Query for query parameters
        var message = req.Query["message"].ToString();

        // Fix for 'Content' error: Use req.Body to read the request body
        if (string.IsNullOrEmpty(message))
        {
            using (var reader = new StreamReader(req.Body))
            {
                message = await reader.ReadToEndAsync();
            }
        }

        if (string.IsNullOrEmpty(message))
        {
            _logger.LogError("No message provided.");
            return new BadRequestObjectResult("No message provided.");
        }      

      

        string url = $"{endpoint}openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";

        using (HttpClient client = new HttpClient())
        {
            // Set headers
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("api-key", key);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Prepare the request body
            var requestBody = new
            {
                messages = new object[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = "Write a function for binary search in C#." }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);

            // Send the request
            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                string answer = jsonResponse.choices[0].message.content;
                return new OkObjectResult(answer);
            }
            else
            {
                return new OkObjectResult(await response.Content.ReadAsStringAsync());
            }

           
        }
    }

    [Function("MCPAgentSDK")]
    public async Task<IActionResult> MCPAgentSDK([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
        ChatClient chatClient = azureClient.GetChatClient(deploymentName);

        // Get chat completion
        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage("write a function for binary search in c#"),
        ]);

        //ChatCompletion completion = chatClient.CompleteChat(
        //[
        // System messages represent instructions or other guidance about how the assistant should behave
        //new SystemChatMessage("You are a helpful assistant that talks like a pirate."),
        // User messages represent user input, whether historical or the most recent input
        //new UserChatMessage("Hi, can you help me?"),
        // Assistant messages in a request represent conversation history for responses
        //new AssistantChatMessage("Arrr! Of course, me hearty! What can I do for ye?"),
        //new UserChatMessage("What's the best way to train a parrot?"),
        //]);


        return new OkObjectResult(completion.Content[0].Text);
    }
}