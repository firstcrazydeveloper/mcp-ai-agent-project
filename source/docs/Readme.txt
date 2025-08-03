#Install Azure Function for VS Code: 

npm install -g azure-functions-core-tools@4 --unsafe-perm true

#Go to src folder >> Run command:

1. dotnet clean ./MCPAIAgentProject 
2. dotnet build ./MCPAIAgentProject
3. func start --script-root ./MCPAIAgentProject --verbose

#For VSCode Debugging
In Terminal find the Process Id and update that in launch.json in configurations section and then from Run & Debug attach the "Attach to Isolated Worker" Process



#Docker command

docker build  -f MCPAIAgentProject/Dockerfile -t my-azure-functionv1.1 .
docker run -p 8080:80 --name myfunctiondebug my-azure-functionv1.1


This may be a liittle bit too late but this command can be run even when pip path is not set. I am using Python 3.7 running on Windows 10 and this is the command

py -m pip install requests