#Install Azure Function for VS Code: 

npm install -g azure-functions-core-tools@4 --unsafe-perm true

#Go to src folder >> Run command:

1. dotnet clean ./MCPAIAgentProject 
2. dotnet build ./MCPAIAgentProject
3. func start --script-root ./MCPAIAgentProject --verbose

#For VSCode Debugging
In Terminal find the Process Id and update that in launch.json in configurations section and then from Run & Debug attach the "Attach to Isolated Worker" Process



#Docker command

1. docker run  mcpagentfunctionv1.0
2. docker run  mcpagentfunctionv1.0