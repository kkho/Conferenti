# Using Ollama with Conferenti AI Agent

This guide explains how to use Ollama as a local LLM backend for the Conferenti AI Agent.

## Why Ollama?

- **Real AI responses** - Actual LLM inference, not mocked
- **100% local** - No API costs, works offline
- **OpenAI compatible** - Drop-in replacement
- **Fast iteration** - No rate limits or quotas
- **Privacy** - Your data never leaves your machine

## Setup Instructions

### 1. Install Ollama

**Windows:**

```powershell
# Download from: https://ollama.com/download
# Or use winget:
winget install Ollama.Ollama
```

**Mac:**

```bash
brew install ollama
```

**Linux:**

```bash
curl -fsSL https://ollama.com/install.sh | sh
```

### 2. Pull a Model

```powershell
# Recommended for development (fast, 3B params)
ollama pull llama3.2

# Or other options:
ollama pull llama3.1      # 8B params, more capable
ollama pull mistral       # 7B params, good balance
ollama pull phi3          # Small and efficient
ollama pull codellama     # Code-focused
```

### 3. Verify Ollama is Running

```powershell
ollama list                # List installed models
ollama serve              # Start server (usually auto-starts)
```

Ollama runs at: `http://localhost:11434`

### 4. Configure Your Agent

Copy `.env.example` to `.env`:

```powershell
cp .env.example .env
```

Edit `.env`:

```env
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
API_KEY=ollama
```

### 5. Install Python Dependencies

```powershell
cd conferenti-ai-agent
pip install -r requirements.txt
```

This includes:

- `ollama==0.4.7` - Official Ollama Python client
- `azure-ai-agents` - Compatibility layer

### 6. Run the Agent

```powershell
python -m conferenti_agent.main
```

You should see:

```
🤖 Starting Conferenti AI Agent...
📍 Endpoint: http://localhost:11434/v1
🎯 Model: llama3.2
✅ Agent 'conferenti-agent' created successfully!

💬 Starting interactive chat (type 'exit' to quit, 'clear' to reset):

You:
```

## Usage Examples

### Basic Chat

```
You: What is Conferenti?
Agent: Conferenti is a conference system...

You: List some popular tech sessions
Agent: Here are some popular tech sessions...
```

### Commands

- Type `exit` or `quit` to stop
- Type `clear` to reset conversation history

## Model Comparison

| Model         | Size | Speed  | Quality  | Best For                |
| ------------- | ---- | ------ | -------- | ----------------------- |
| **llama3.2**  | 3B   | ⚡⚡⚡ | ⭐⭐⭐   | Development, testing    |
| **llama3.1**  | 8B   | ⚡⚡   | ⭐⭐⭐⭐ | Production-like quality |
| **mistral**   | 7B   | ⚡⚡   | ⭐⭐⭐⭐ | Balanced performance    |
| **phi3**      | 3.8B | ⚡⚡⚡ | ⭐⭐⭐   | Resource-constrained    |
| **codellama** | 7B   | ⚡⚡   | ⭐⭐⭐⭐ | Code generation         |

## Switching Between Backends

The adapter automatically detects which backend to use based on `PROJECT_ENDPOINT`:

### Use Ollama (Local)

```env
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
```

### Use Azure OpenAI (Production)

```env
PROJECT_ENDPOINT=https://your-resource.openai.azure.com
MODEL_DEPLOYMENT_NAME=gpt-4
API_KEY=your-azure-api-key
```

### Use aoai-api-simulator (Mock)

```env
PROJECT_ENDPOINT=http://localhost:8000
MODEL_DEPLOYMENT_NAME=gpt-35-turbo-10k-token
API_KEY=gWRWvXXwpXkjvYiuNz55MYjtW_tmmog
```

## Programmatic Usage

```python
from conferenti_agent.ollama_adapter import create_agent_client

# Create client (auto-detects backend)
client = create_agent_client()

# Create agent
agent = client.create_agent(
    name="my-agent",
    instructions="You are a helpful assistant"
)

# Get response
response = agent.run("Hello!")
print(response["content"])

# Streaming
for chunk in agent.run("Tell me a story", stream=True):
    if chunk["status"] == "in_progress":
        print(chunk["delta"], end="", flush=True)
```

## Troubleshooting

### "Connection refused" error

```powershell
# Start Ollama server
ollama serve
```

### "Model not found" error

```powershell
# Pull the model first
ollama pull llama3.2
```

### Slow responses

- Try a smaller model (`llama3.2` instead of `llama3.1`)
- Close other applications
- Check GPU availability: `ollama ps`

### Out of memory

```powershell
# Use a smaller model
ollama pull phi3
```

Then update `.env`:

```env
MODEL_DEPLOYMENT_NAME=phi3
```

## Advanced Configuration

### Custom Ollama Host

```env
PROJECT_ENDPOINT=http://192.168.1.100:11434/v1
```

### Model Parameters

Edit `agent.py` to customize:

- Temperature
- Top-p
- Max tokens
- Stop sequences

## Performance Tips

1. **First run is slow** - Model loads into memory
2. **Subsequent runs are fast** - Model stays cached
3. **Use GPU** - 10-100x faster than CPU
4. **Smaller models** - Better for development/testing
5. **Keep Ollama running** - Avoid cold start delays
