namespace MCPAIAgentProject
{
    public interface IMCPAPIAgentConfig
    {
        string Endpoint { get; }
        string Key { get; }
        string DeploymentName { get; }
        string ApiVersion { get; }
    }
}
