using MCPAIAgentProject.Models;
using MCPAIAgentProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MCPAIAgentProject;

public class MCPAgentFunction
{
    private readonly ILogger<MCPAgentFunction> _logger;
    private readonly IAzureOpenAIService _aiService;

    public MCPAgentFunction(ILogger<MCPAgentFunction> logger, IAzureOpenAIService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    [Function("MCPAgentFunctionAPI")]
    public async Task<IActionResult> MCPAgentFunctionAPI([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Processing AI chat request.");

        string message = req.Query["message"].ToString();

        if (req.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            AIRequest request = new AIRequest
            {
                Message = message
            };

            message = JsonConvert.SerializeObject(request);
        }

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
       

        var aiRequest = JsonConvert.DeserializeObject<AIRequest>(message);
        if (aiRequest == null || string.IsNullOrWhiteSpace(aiRequest.Message))
        {
            return new BadRequestObjectResult("Invalid request. Message is required.");
        }

        var response = await _aiService.GetAPIChatCompletionAsync(aiRequest);
        return new OkObjectResult(response);
    }


    [Function("MCPAgentFunctionSDK")]
    public async Task<IActionResult> MCPAgentFunctionSDK([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Processing AI chat request.");

        string message = req.Query["message"].ToString();

        if (req.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            AIRequest request = new AIRequest
            {
                Message = message
            };

            message = JsonConvert.SerializeObject(request);
        }

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


        var aiRequest = JsonConvert.DeserializeObject<AIRequest>(message);
        if (aiRequest == null || string.IsNullOrWhiteSpace(aiRequest.Message))
        {
            return new BadRequestObjectResult("Invalid request. Message is required.");
        }

        var response = await _aiService.GetSDKChatCompletionAsync(aiRequest);
        return new OkObjectResult(response);
    }
}