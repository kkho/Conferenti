# 🚀 Quick Start Guide - Conferenti AI Agent

This guide will get you up and running in 5 minutes!

## Prerequisites

✅ Python 3.11 or higher  
✅ PowerShell (Windows) or Terminal (Mac/Linux)  
✅ Internet connection (for initial setup)

## Step-by-Step Instructions

### 1️⃣ Install Python Dependencies

Open PowerShell and run:

```powershell
cd d:\Hobby\Conferenti\conferenti-ai-agent
pip install -r requirements.txt
```

Wait for all packages to install (~2-3 minutes).

---

### 2️⃣ Choose Your AI Backend

You have 3 options. Pick **ONE**:

#### 🟢 Option A: Ollama (RECOMMENDED - Free, Local, Fast)

**Install Ollama:**

```powershell
winget install Ollama.Ollama
```

**Pull a model:**

```powershell
ollama pull llama3.2
```

**Create `.env` file** (copy from `.env.example`):

```env
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
API_KEY=ollama
LOG_LEVEL=INFO
```

---

#### 🔵 Option B: Azure OpenAI (Production - Requires Azure Account)

**Create `.env` file:**

```env
PROJECT_ENDPOINT=https://your-resource.openai.azure.com
MODEL_DEPLOYMENT_NAME=gpt-4
API_KEY=your-azure-api-key
LOG_LEVEL=INFO
```

Replace `your-resource`, and `your-azure-api-key` with your actual values.

---

#### 🟡 Option C: aoai-api-simulator (Testing - Mock Responses)

**Start simulator in one terminal:**

```powershell
cd aoai-api-simulator
pip install -r requirements.txt
python -m uvicorn aoai_api_simulator.main:app --host 0.0.0.0 --port 8000
```

**Copy API key from startup logs** (looks like `gWRWvXXwpXkjvYiuNz55MYjtW_tmmog`)

**Create `.env` file in another terminal:**

```env
PROJECT_ENDPOINT=http://localhost:8000
MODEL_DEPLOYMENT_NAME=gpt-35-turbo-10k-token
API_KEY=<paste-api-key-here>
LOG_LEVEL=INFO
```

---

### 3️⃣ Run the Agent

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

---

### 4️ Try It Out!

Type your questions and press Enter:

```
You: Hello! What can you help me with?
Agent: Hello! I'm your Conferenti AI assistant. I can help you with...

You: Suggest 3 speakers for a Python conference
Agent: Here are 3 excellent speaker suggestions for a Python conference...

You: exit
👋 Goodbye!
```

---

## Available Commands

| Command           | Description                |
| ----------------- | -------------------------- |
| `<your question>` | Chat with the agent        |
| `clear`           | Reset conversation history |
| `exit` or `quit`  | Stop the agent             |
| `Ctrl+C`          | Force quit                 |

---

## Troubleshooting

### "ModuleNotFoundError: No module named 'conferenti_agent'"

**Solution:**

```powershell
pip install -r requirements.txt
```

---

### "Connection refused" (Using Ollama)

**Check if Ollama is running:**

```powershell
ollama list
```

**Start Ollama:**

```powershell
ollama serve
```

---

### "Model not found" (Using Ollama)

**Pull the model:**

```powershell
ollama pull llama3.2
```

---

### ".env file not found"

**Create it from example:**

```powershell
Copy-Item .env.example .env
```

Then edit `.env` with your settings.

---

### Agent gives poor responses

**Try a better model:**

```powershell
ollama pull llama3.1  # Larger, smarter model
```

Update `.env`:

```env
MODEL_DEPLOYMENT_NAME=llama3.1
```

---

### For API Integration

Check [OLLAMA_SETUP.md](./OLLAMA_SETUP.md) for:

- REST API endpoints
- Integration with Next.js
- Production deployment

---

## 💡 Quick Examples

### Example 1: Ask about conferences

```
You: What makes a good conference speaker?
```

### Example 2: Get suggestions

```
You: Suggest 5 session topics for a DevOps conference
```

### Example 3: Planning help

```
You: How should I organize a 2-day tech conference?
```

---

## ✅ Verification Checklist

Before asking for help, verify:

- [ ] Python 3.11+ installed: `python --version`
- [ ] Dependencies installed: `pip list | Select-String conferenti`
- [ ] `.env` file exists: `Test-Path .env`
- [ ] Backend is running (Ollama/Azure/Simulator)
- [ ] No firewall blocking localhost connections

---

## 🆘 Still Having Issues?

1. **Check all terminals** - Make sure simulator isn't running if using Ollama
2. **Restart PowerShell** - Sometimes environment variables need refresh
3. **Clear Python cache:**
   ```powershell
   Get-ChildItem -Recurse -Directory __pycache__ | Remove-Item -Recurse -Force
   ```
4. **Reinstall dependencies:**
   ```powershell
   pip uninstall -y conferenti-agent
   pip install -r requirements.txt
   ```

---

## 🎉 Success!

If you see the chat prompt, you're all set! Start chatting with your AI agent.

**Have fun!** 🚀
