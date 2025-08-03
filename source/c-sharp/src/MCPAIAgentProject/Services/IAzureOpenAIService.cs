using MCPAIAgentProject.Models;

namespace MCPAIAgentProject.Services
{
    public interface IAzureOpenAIService
    {
        Task<AIResponse> GetAPIChatCompletionAsync(AIRequest request);
        Task<AIResponse> GetSDKChatCompletionAsync(AIRequest request);
    }
}
