namespace MCPAIAgentProject
{
    public class MCPAPIAgentConfig : IMCPAPIAgentConfig
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string DeploymentName { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = "2025-01-01-preview";
    }
}
