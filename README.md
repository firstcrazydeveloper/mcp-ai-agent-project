
# MCP AI Agent Project

A **.NET 9 Azure Function** using **Azure OpenAI** with:
- **Dependency Injection**
- **Configuration via `local.settings.json`**
- **Strongly-typed models**
- **Retry logic (Polly)**
- **Streaming token-by-token AI responses**

---

## **1. Prerequisites**
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


---

## **7. API Endpoints**

The following HTTP endpoints are available in this Azure Function App:

### **1. MCPAgentAPI**
- **URL:** `/api/MCPAgentAPI`
- **Methods:** `GET`, `POST`
- **Request Body (POST):**
```json
{
  "message": "Write a function for binary search in C#."
}
```
- **Response:**
```json
{
  "response": "public int BinarySearch(int[] arr, int target) { ... }"
}
```

### **2. MCPAgentFunctionAPI**
- **URL:** `/api/MCPAgentFunctionAPI`
- **Methods:** `GET`, `POST`
- **Request Body (POST):**
```json
{
  "message": "Explain how binary search works."
}
```
- **Response:**
```json
{
  "response": "Binary search is an algorithm that finds the position of a target value within a sorted array..."
}
```

### **3. MCPAgentFunctionSDK**
- **URL:** `/api/MCPAgentFunctionSDK`
- **Methods:** `GET`, `POST`
- **Request Body (POST):**
```json
{
  "message": "Write a function for quicksort in C#."
}
```
- **Response:**
```json
{
  "response": "public void QuickSort(int[] arr, int left, int right) { ... }"
}
```

### **4. MCPAgentSDK**
- **URL:** `/api/MCPAgentSDK`
- **Methods:** `GET`, `POST`
- **Request Body (POST):**
```json
{
  "message": "Write a function for merge sort in C#."
}
```
- **Response:**
```json
{
  "response": "public void MergeSort(int[] arr) { ... }"
}
```

**Note:**  
- All endpoints accept a `message` in the request body (for `POST`).
- Responses are AI-generated content using Azure OpenAI GPT models.

