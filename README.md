# Semantic Kernel Chat Assistant 🤖💬

![C#](https://img.shields.io/badge/Language-C%23-blue) ![Azure](https://img.shields.io/badge/Cloud-Azure-blue) ![OpenAI](https://img.shields.io/badge/OpenAI-GPT--4-green)

## Description

This project is a **chat assistant** built using the **Microsoft Semantic Kernel** framework. It integrates with **Azure OpenAI** services to provide intelligent conversational capabilities.  
👨‍💻 ☁️ ⚛️

## How it works:

[![Example](https://img.youtube.com/vi/HC8f0mpYa3M/0.jpg)](https://www.youtube.com/watch?v=HC8f0mpYa3M "Example")

## How to Run the Project

1. **Clone the Repository** 🛠️
```bash
   git clone https://github.com/your-repo-name.git
   cd your-repo-name
```

2. **Install Dependencies** 💾  
   Ensure you have .NET 9.0 installed.  
   You can download it from the [.NET website](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
dotnet restore
```

3. **Configure appsettings.json** ✏️  
   Update the OpenAi section with your Azure OpenAI credentials:
- ApiKey: Your Azure OpenAI API key.
- ApiEndpoint: The endpoint URL for your Azure OpenAI service.
- DeploymentName: The deployment name for your GPT model.

**appsettings.json Structure:**
```json
{
  "OpenAi": {
    "ApiKey": "<API_KEY>",
    "ApiEndpoint": "<API_ENDPOINT>",
    "DeploymentName": "<DEPLOYMENT_NAME>"
  }
}
```

4. **Run the Application** ▶️
```bash
   dotnet run
```

### References:
* [@daraujo85](https://github.com/daraujo85/poc-semantic-kernel).
* [@DevLeader](https://www.youtube.com/@DevLeader)
