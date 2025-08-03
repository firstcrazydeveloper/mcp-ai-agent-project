
# MCP AI Agent Project

A **.NET 9 Azure Function** using **Azure OpenAI** with:
- **Dependency Injection**
- **Configuration via `local.settings.json`**
- **Strongly-typed models**
- **Retry logic (Polly)**
- **Streaming token-by-token AI responses**

---

## **1. Prerequisites**
- [Azure OpenAI Service](https://portal.azure.com/)
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) (for Functions Core Tools)
- [Docker](https://www.docker.com/)
- [VS Code](https://code.visualstudio.com/) with **Azure Functions** extension

---

## **2. Install Azure Functions Core Tools**

Install the Azure Functions CLI globally:

```bash
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```

---

## **3. Running Locally**

### **Step 1 – Clean & Build**
Go to the `src` folder and run:

```bash
dotnet clean ./MCPAIAgentProject 
dotnet build ./MCPAIAgentProject
```

### **Step 2 – Start the Function Host**
Run with verbose logging:

```bash
func start --script-root ./MCPAIAgentProject --verbose
```

Your functions will be available at:

```
http://localhost:7071/api/<FunctionName>
```

---

## **4. VS Code Debugging**

1. Start your function:
   ```bash
   func start --script-root ./MCPAIAgentProject --dotnet-isolated-debug
   ```
2. In VS Code terminal, **find the worker process ID** (look for `dotnet` process).
3. Update the `processId` in `.vscode/launch.json` under the `Attach to Isolated Worker` configuration:
   ```json
   {
       "name": "Attach to Isolated Worker",
       "type": "coreclr",
       "request": "attach",
       "processId": "12345"
   }
   ```
4. Go to **Run & Debug** → Select **"Attach to Isolated Worker"** → Press **F5**.

---

## **5. Docker Commands**

### **Build & Run:**
```bash
docker build -t mcpagentfunctionv1.0 .
docker run -p 8080:80 mcpagentfunctionv1.0
```

Then access:
```
http://localhost:8080/api/<FunctionName>
```

---

## **6. Configuration**

### **Option A – As an Object**
Use a strongly-typed section in `local.settings.json`:
```json
"AzureOpenAI": {
  "Endpoint": "https://your-endpoint.openai.azure.com/",
  "Key": "your-key",
  "DeploymentName": "gpt-35-turbo",
  "ApiVersion": "2025-01-01-preview"
}
```

### **Option B – As Values**
Use nested values (helpful when Docker/env overrides are used):
```json
"Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureOpenAI:Endpoint": "https://your-endpoint.openai.azure.com/",
    "AzureOpenAI:Key": "your-key",
    "AzureOpenAI:DeploymentName": "gpt-35-turbo",
    "AzureOpenAI:ApiVersion": "2025-01-01-preview",
    "Host:LocalHttpPort": "7072"
}
```

**Note:**  
If a value is not found in these configs, the application will throw an error to prevent running with missing settings.
