import azure.functions as func
import logging
import requests as re
import sys
import json


print(sys.version)

def load_settings():
    # Open and read the settings.json file
    with open('local.settings.json', 'r') as file:
        settings = json.load(file)
    return settings

# Load settings from the settings file
settings = load_settings()

api_key = settings['api_key']
endpoint = settings['endpoint']
model = settings['model']

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

@app.route(route="MCPAIAgentAPI")
def MCPAIAgentAPI(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    message = req.params.get('message')
    
    if not message:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('message')
    logging.info('Abhishek.')
    messages = [
        {
            "role": "system",
            "content": "You are a helpful assistant.",
        },
        {
            "role": "user",
            "content": 'Write a function for binary search in C#',
        }
    ]
    headers = {
        'Authorization': f'Bearer {api_key}',
        'Content-Type': 'application/json'
    }
    
    data = {
        "messages": messages,
        "max_tokens": 4096,
        "temperature": 1.0,
        "top_p": 1.0,
        "model": model
    }
    
    response = re.post(endpoint, headers=headers, json=data)
    logging.info('Abhishek 2.')
    logging.info(response.status_code)
    logging.info(response.text)
    if response.status_code == 200:
        result = response.json()
        agent_response = result['choices'][0]['message']['content']
        logging.info(f"Agent response: {agent_response}")
        return func.HttpResponse(agent_response, status_code=200)
    else:
        return func.HttpResponse("Error processing your request.", status_code=500)
    if name:
        return func.HttpResponse(f"Hello, {name}. This HTTP triggered function executed successfully.")
    else:
        return func.HttpResponse(
             "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.",
             status_code=200
        )