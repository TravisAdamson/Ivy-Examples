using Ivy;

namespace OpenAIExample.Apps
{
    [App(icon: Icons.MessageCircle, title: "OpenAI Demo")]
    public class OpenAIExample : ViewBase
    {
        public override object? Build()
        {
            var userInput = UseState("");
            var conversation = UseState(new List<ChatMessage>());
            var isLoading = UseState(false);

            return Layout.Vertical().Gap(3).Padding(4)
                | Text.H1("OpenAI Mock Demo").Color(Colors.Blue)
                | Text.P("This demo simulates OpenAI responses without requiring an API key.").Color(Colors.Gray)

                // Chat Interface
                | new Card(
                    Layout.Vertical().Gap(3)
                        | (conversation.Value.Count == 0
                            ? Text.P("Start a conversation below!").Color(Colors.Gray)
                            : conversation.Value.Select(msg =>
                                Layout.Horizontal().Gap(2)
                                    | new Avatar(msg.Role == "user" ? "You" : "AI", msg.Role == "user" ? "U" : "AI")
                                    | new Card(Text.Literal(msg.Content)).Width(Size.Grow())
                            ))
                        | (isLoading.Value ? new Progress((int?)null) : null)
                ).Height(50).Width(Size.Full())

                // Input Section
                | Layout.Horizontal().Gap(2).Width(Size.Full())
                    | new TextInput(userInput)
                              .Placeholder("Ask me anything...")
                              .Width(Size.Grow())
                              .Disabled(isLoading.Value)
                    | new Button("Send", onClick: async _ => await SendMockMessage(userInput, conversation, isLoading))
                              .Icon(Icons.Send)
                              .Disabled(isLoading.Value || string.IsNullOrEmpty(userInput.Value))

                // Example Prompts
                | new Card(
                    Layout.Vertical().Gap(2)
                        | Text.H4("Example prompts:")
                        | Layout.Wrap().Gap(1)
                            | new Button("Hello!", onClick: _ => userInput.Set("Hello!"))
                                  .Variant(ButtonVariant.Outline)
                                  .Small()
                            | new Button("Clear", onClick: _ => conversation.Set(new List<ChatMessage>()))
                                  .Variant(ButtonVariant.Ghost)
                                  .Small()
                );
        }

        private async ValueTask SendMockMessage(
            IState<string> userInput,
            IState<List<ChatMessage>> conversation,
            IState<bool> isLoading)
        {
            isLoading.Set(true);

            try
            {
                var userMessage = new ChatMessage { Role = "user", Content = userInput.Value };
                var updatedConversation = conversation.Value.ToList();
                updatedConversation.Add(userMessage);

                userInput.Set("");

                // Simulate API delay
                await Task.Delay(1000);

                // Generate mock response
                var mockResponse = userMessage.Content.ToLower() switch
                {
                    string s when s.Contains("hello") => "Hello! I'm an AI assistant. How can I help you today?",
                    string s when s.Contains("weather") => "I'm an AI, so I don't have real-time weather data, but I can help you find weather information if you tell me your location!",
                    string s when s.Contains("c#") || s.Contains("code") => "Here's a simple C# example:\n\n```csharp\npublic class Program\n{\n    public static void Main()\n    {\n        Console.WriteLine(\"Hello, World!\");\n    }\n}```",
                    _ => $"I understand you're asking about: {userMessage.Content}. This is a mock response - in a real app, this would call the OpenAI API."
                };

                var aiMessage = new ChatMessage { Role = "assistant", Content = mockResponse };
                updatedConversation.Add(aiMessage);
                conversation.Set(updatedConversation);
            }
            finally
            {
                isLoading.Set(false);
            }
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }
}