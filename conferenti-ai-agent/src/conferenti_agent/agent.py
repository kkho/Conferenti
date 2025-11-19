import os
from typing import Optional, List, Dict, Any
import asyncio
import ollama
from openai import AzureOpenAI
from azure.ai.agents import AgentsClient
from azure.identity import DefaultAzureCredential
from azure.core.credentials import AzureKeyCredential


class ConferentiAgentAdapter:
    """
    Allows seamless switching between Azure and local Ollama for development.
    """

    def __init__(
        self,
        model: str = "llama3.2",
        base_url: str = "http://localhost:11434",
        use_ollama: bool = True,
    ):
        """
        Initialize adapter.

        Args:
            model: Ollama model name (e.g., 'llama3.2', 'mistral')
            base_url: Ollama server URL
            use_ollama: If False, falls back to real Azure AI Agents
        """
        self.model = model
        self.base_url = base_url
        self.use_ollama = use_ollama
        self.conversation_history: List[Dict[str, str]] = []

        # If not using Ollama, initialize real Azure client
        if not use_ollama:
            self.azure_client = AgentsClient(
                endpoint=os.environ["PROJECT_ENDPOINT"],
                credential=AzureKeyCredential(os.environ["API_KEY"]),
            )

    def create_agent(
        self,
        name: str,
        instructions: str,
        tools: Optional[List[Dict[str, Any]]] = None,
        **kwargs
    ) -> "AiAgent":
        """
        Create an agent (mimics Azure AI Agents SDK).

        Args:
            name: Agent name
            instructions: System instructions for the agent
            tools: Optional list of tools/functions the agent can use

        Returns:
            AiAgent instance
        """
        if not self.use_ollama:
            # Fall back to real Azure implementation
            return self.azure_client.create_agent(
                model=self.model,
                name=name,
                instructions=instructions,
                tools=tools,
                **kwargs
            )

        return AiAgent(
            model=self.model,
            name=name,
            instructions=instructions,
            tools=tools,
            base_url=self.base_url,
        )


class AiAgent:
    """
    Agent implementation using Ollama.
    Compatible with Azure AI Agents interface.
    """

    def __init__(
        self,
        model: str,
        name: str,
        instructions: str,
        tools: Optional[List[Dict[str, Any]]] = None,
        base_url: str = "http://localhost:11434",
    ):
        self.model = model
        self.name = name
        self.instructions = instructions
        self.tools = tools or []
        self.base_url = base_url
        self.conversation_history: List[Dict[str, str]] = []

        # Initialize with system instructions
        self.conversation_history.append({"role": "system", "content": instructions})

    def run(self, message: str, stream: bool = False) -> Dict[str, Any]:
        """
        Run the agent with a user message.

        Args:
            message: User input message
            stream: Whether to stream the response

        Returns:
            Agent response
        """

        self.conversation_history.append({"role": "user", "content": message})

        if stream:
            return self.run_streaming(message)
        else:
            return self.run_sync(message)

    def run_sync(self, message: str) -> Dict[str, Any]:
        """Synchronous execution."""
        try:
            # Create Ollama client with custom host
            client = ollama.Client(host=self.base_url)
            response = client.chat(model=self.model, messages=self.conversation_history)

            assistant_message = response["message"]["content"]
            self.conversation_history.append(
                {"role": "assistant", "content": assistant_message}
            )

            return {
                "status": "completed",
                "content": assistant_message,
                "model": self.model,
                "message": response["message"],
            }

        except Exception as e:
            return {"status": "failed", "error": str(e)}

    def run_streaming(self, message: str):
        """Streaming execution (generator)."""
        try:
            full_response = ""

            # Create Ollama client with custom host
            client = ollama.Client(host=self.base_url)

            for chunk in client.chat(
                model=self.model, messages=self.conversation_history, stream=True
            ):
                content = chunk["message"]["content"]
                full_response += content

                yield {"status": "in_progress", "content": content, "delta": content}

            # Add complete response to history
            self.conversation_history.append(
                {"role": "assistant", "content": full_response}
            )
        except Exception as e:
            yield {"status": "failed", "error": str(e)}

    def clear_history(self):
        """Clear conversation history except system instructions."""
        system_msg = self.conversation_history[0]
        self.conversation_history = [system_msg]

    def get_history(self) -> List[Dict[str, str]]:
        """Get conversation history."""
        return self.conversation_history.copy()


def create_agent_client(use_ollama: bool = None) -> ConferentiAgentAdapter:
    """
    Factory function to create agent client.
    Automatically detects if Ollama should be used based on environment.

    Args:
        use_ollama: Override automatic detection

    Returns:
        ConferentiAgentAdapter instance
    """

    endpoint = os.getenv("PROJECT_ENDPOINT", "http://localhost:11434/v1")

    # Auto-detect: use Ollama if endpoint contains localhost or host.docker.internal
    if use_ollama is None:
        use_ollama = any(
            x in endpoint.lower()
            for x in ["localhost", "127.0.0.1", "host.docker.internal", "ollama"]
        )

    model = os.getenv("MODEL_DEPLOYMENT_NAME", "llama3.2")

    # Extract base URL for Ollama (remove /v1 suffix if present)
    base_url = endpoint.replace("/v1", "") if "/v1" in endpoint else endpoint

    return ConferentiAgentAdapter(model=model, base_url=base_url, use_ollama=use_ollama)
