namespace MCPAIAgentProject
{
    public interface IAzureOpenAIConfig
    {
        string Endpoint { get; }
        string Key { get; }
        string DeploymentName { get; }
        string ApiVersion { get; }
    }
}
