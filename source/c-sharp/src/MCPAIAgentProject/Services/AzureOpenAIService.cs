using Azure;
using Azure.AI.OpenAI;
using MCPAIAgentProject.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Chat;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text;

namespace MCPAIAgentProject.Services
{
    public class AzureOpenAIService : IAzureOpenAIService
    {
        private readonly IAzureOpenAIConfig _config;
        private readonly ILogger<AzureOpenAIService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public AzureOpenAIService(IAzureOpenAIConfig config, ILogger<AzureOpenAIService> logger, HttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;

            // Configure retry policy: 3 retries with exponential backoff
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} for {result.Result.StatusCode}. Waiting {timeSpan.TotalSeconds}s.");
                    });
        }

        public async Task<AIResponse> GetAPIChatCompletionAsync(AIRequest request)
        {
            string url = $"{_config.Endpoint}openai/deployments/{_config.DeploymentName}/chat/completions?api-version={_config.ApiVersion}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _config.Key);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                messages = new object[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = request.Message }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _retryPolicy.ExecuteAsync(ct =>
                _httpClient.PostAsync(url, httpContent, ct), CancellationToken.None);

            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error from Azure OpenAI: {response.StatusCode} - {result}");
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode}");
            }

            dynamic jsonResponse = JsonConvert.DeserializeObject(result);
            string answer = jsonResponse?.choices[0]?.message?.content ?? "No response from AI.";

            return new AIResponse { Answer = answer };
        }


        public async Task<AIResponse> GetSDKChatCompletionAsync(AIRequest request)
        {
            AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));
            ChatClient chatClient = azureClient.GetChatClient(_config.DeploymentName);
            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 500,  // same as max_tokens
                Temperature = 0.7f           // controls creativity
            };

            // Get chat completion
            ChatCompletion completion = await chatClient.CompleteChatAsync(
            [
                new SystemChatMessage("You are a helpful assistant."),
                new UserChatMessage(request.Message),

            ], options);

            


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

            if (completion == null)
            {
                _logger.LogError($"Error from Azure OpenAI SDK Call ");
                throw new HttpRequestException($"OpenAI SDK call error");
            }
            
            string answer = completion?.Content[0]?.Text ?? "No response from AI.";
            return new AIResponse { Answer = answer };
        }
    }
}
