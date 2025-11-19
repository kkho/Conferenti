"""
Main entry point for Conferenti AI Agent.
"""

import asyncio
from dotenv import load_dotenv
from conferenti_agent.config import get_settings
from conferenti_agent.agent import create_agent_client


async def main():
    """Main application entry point."""
    # Load environment variables
    load_dotenv()

    # Get configuration
    settings = get_settings()

    print("🤖 Starting Conferenti AI Agent...")
    print(f"📍 Endpoint: {settings.project_endpoint}")
    print(f"🎯 Model: {settings.model_deployment_name}")

    # Create agent client (auto-detects Ollama vs Azure)
    client = create_agent_client()

    # Create the agent
    agent = client.create_agent(
        name="conferenti-agent",
        instructions="""You are a helpful AI assistant for the Conferenti session management system.
        
Your responsibilities:
- Help users find information about speakers and sessions
- Answer questions about session schedules
- Provide recommendations for relevant talks
- Assist with session planning and organization

Be concise, friendly, and professional in all interactions.""",
    )

    print(f"✅ Agent '{agent.name}' created successfully!")
    print("\n💬 Starting interactive chat (type 'exit' to quit, 'clear' to reset):\n")

    # Interactive loop
    while True:
        try:
            user_input = input("You: ").strip()

            if not user_input:
                continue

            if user_input.lower() in ["exit", "quit"]:
                print("\n👋 Goodbye!")
                break

            if user_input.lower() == "clear":
                agent.clear_history()
                print("\n🧹 Conversation history cleared.\n")
                continue

            print("Agent: ", end="", flush=True)

            # Run agent with streaming
            full_response = ""
            for chunk in agent.run(user_input, stream=True):
                if chunk["status"] == "in_progress":
                    print(chunk["delta"], end="", flush=True)
                    full_response += chunk["delta"]
                elif chunk["status"] == "failed":
                    print(f"\n❌ Error: {chunk['error']}")
                    break

            print("\n")  # New line after response

        except KeyboardInterrupt:
            print("\n\n👋 Goodbye!")
            break
        except Exception as e:
            print(f"\n❌ Error: {e}\n")


if __name__ == "__main__":
    asyncio.run(main())
