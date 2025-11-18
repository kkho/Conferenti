# Command Cheat Sheet - Conferenti AI Agent

## 🚀 Quick Commands

### Installation

```powershell
# Install dependencies
pip install -r requirements.txt
```

### Setup Ollama (Recommended)

```powershell
# Install Ollama
winget install Ollama.Ollama

# Pull model
ollama pull llama3.2

# Verify
ollama list
```

### Create .env File

```powershell
# Copy from example
Copy-Item .env.example .env

# Edit with notepad
notepad .env
```

### Run the Agent

```powershell
# Start interactive chat
python -m conferenti_agent.main
```

---

## 📝 .env Configuration Examples

### For Ollama (Local)

```env
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
API_KEY=ollama
LOG_LEVEL=INFO
```

### For Azure OpenAI (Production)

```env
PROJECT_ENDPOINT=https://your-resource.openai.azure.com
MODEL_DEPLOYMENT_NAME=gpt-4
API_KEY=your-azure-api-key
LOG_LEVEL=INFO
```

### For Simulator (Testing)

```env
PROJECT_ENDPOINT=http://localhost:8000
MODEL_DEPLOYMENT_NAME=gpt-35-turbo-10k-token
API_KEY=<get-from-simulator-logs>
LOG_LEVEL=INFO
```

---

## 🎮 In-Chat Commands

| Command          | Action             |
| ---------------- | ------------------ |
| `<type message>` | Chat with agent    |
| `clear`          | Reset conversation |
| `exit`           | Quit agent         |
| `quit`           | Quit agent         |
| `Ctrl+C`         | Force quit         |

---

## 🔧 Troubleshooting Commands

### Check Python Version

```powershell
python --version
# Should be 3.11 or higher
```

### Check Installed Packages

```powershell
pip list | Select-String conferenti
```

### Check if .env Exists

```powershell
Test-Path .env
# Should return True
```

### View .env Contents

```powershell
Get-Content .env
```

### Check Ollama Status

```powershell
ollama list              # List models
ollama ps                # Running models
ollama serve             # Start server
```

### Restart Ollama

```powershell
# Stop Ollama from system tray, then:
ollama serve
```

### Clear Python Cache

```powershell
Get-ChildItem -Recurse -Directory __pycache__ | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Filter "*.pyc" | Remove-Item -Force
```

### Reinstall Dependencies

```powershell
pip uninstall -y conferenti-agent ollama
pip install -r requirements.txt
```

---

## 🧪 Running Simulator

### Start Simulator

```powershell
cd aoai-api-simulator
pip install -r requirements.txt
python -m uvicorn aoai_api_simulator.main:app --host 0.0.0.0 --port 8000
```

### Get Simulator API Key

Look for this line in startup logs:

```
🗝️ Simulator api-key: gWRWvXXwpXkjvYiuNz55MYjtW_tmmog
```

---

## 📦 Ollama Model Commands

### Install Different Models

```powershell
ollama pull llama3.2      # 3B - Fast, good for dev
ollama pull llama3.1      # 8B - Better quality
ollama pull mistral       # 7B - Balanced
ollama pull phi3          # 3.8B - Small & efficient
ollama pull codellama     # 7B - Code-focused
```

### Check Model Info

```powershell
ollama show llama3.2
```

### Remove a Model

```powershell
ollama rm llama3.2
```

---

## 🔍 Debugging Commands

### Check if Port is in Use

```powershell
# Check port 11434 (Ollama)
netstat -ano | findstr :11434

# Check port 8000 (Simulator)
netstat -ano | findstr :8000
```

### Test Ollama Endpoint

```powershell
curl http://localhost:11434/api/tags
```

### View Python Import Paths

```powershell
python -c "import sys; print('\n'.join(sys.path))"
```

### Check Package Location

```powershell
python -c "import conferenti_agent; print(conferenti_agent.__file__)"
```

---

## 📁 Project Navigation

### Go to Project Root

```powershell
cd d:\Hobby\Conferenti\conferenti-ai-agent
```

### List Files

```powershell
Get-ChildItem
```

### Open in VS Code

```powershell
code .
```

---

## 🎯 Common Workflows

### Fresh Start (Reset Everything)

```powershell
# 1. Go to project
cd d:\Hobby\Conferenti\conferenti-ai-agent

# 2. Clear cache
Get-ChildItem -Recurse __pycache__ | Remove-Item -Recurse -Force

# 3. Reinstall
pip install -r requirements.txt

# 4. Verify .env exists
Test-Path .env

# 5. Run
python -m conferenti_agent.main
```

### Switch from Simulator to Ollama

```powershell
# 1. Stop simulator (Ctrl+C)

# 2. Update .env
notepad .env
# Change to:
# PROJECT_ENDPOINT=http://localhost:11434/v1
# MODEL_DEPLOYMENT_NAME=llama3.2
# API_KEY=ollama

# 3. Run agent
python -m conferenti_agent.main
```

### Update Ollama Model

```powershell
# 1. Pull new model
ollama pull llama3.1

# 2. Update .env
notepad .env
# Change MODEL_DEPLOYMENT_NAME=llama3.1

# 3. Run agent
python -m conferenti_agent.main
```

---

## 📚 Documentation Files

| File                       | Description                    |
| -------------------------- | ------------------------------ |
| `QUICKSTART.md`            | 5-minute getting started guide |
| `HOW_TO_RUN.txt`           | Visual step-by-step guide      |
| `README.md`                | Complete documentation         |
| `OLLAMA_SETUP.md`          | Detailed Ollama setup          |
| `AI_SUGGESTIONS_README.md` | AI features documentation      |
| `.env.example`             | Configuration template         |

---

## 💡 Pro Tips

1. **Keep Ollama running** - Faster startup times
2. **Use smaller models** - For development (llama3.2)
3. **Use larger models** - For production (llama3.1, gpt-4)
4. **Check logs** - If something doesn't work
5. **Read error messages** - They usually tell you what's wrong
6. **Test with simulator first** - If Ollama has issues

---

## ✅ Quick Health Check

Run these to verify everything is working:

```powershell
# 1. Check Python
python --version

# 2. Check packages
pip list | Select-String conferenti

# 3. Check .env
Test-Path .env

# 4. Check Ollama (if using)
ollama list

# 5. Run agent
python -m conferenti_agent.main
```

If all pass, you're good to go! 🎉
