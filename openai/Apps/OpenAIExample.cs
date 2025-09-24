using Ivy;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAIExample.Apps
{
    [App(icon: Icons.MessageCircle, title: "OpenAI Chat Demo")]
    public class OpenAIExample : ViewBase
    {
        private const string ApiKey = "";
        private const string Model = "gpt-4o-mini"; // recommended lightweight model

        public override object? Build()
        {
            var userInput = UseState("");
            var conversation = UseState(new List<ChatMessage>());
            var isLoading = UseState(false);

            return Layout.Vertical().Gap(3).Padding(2)
                | Text.H1("OpenAI Chat Demo").Color(Colors.Blue)
                | Text.P("Start chatting with GPT").Color(Colors.Gray)

                // Chat History
                | new Card(
                    Layout.Vertical().Gap(3).Height(40)
                        | (conversation.Value.Count == 0
                            ? Text.Label("No messages yet. Start below!")
                                  .Color(Colors.Gray)
                            : conversation.Value.Select(msg =>
                                Layout.Horizontal().Gap(2).Align(Align.Left)
                                    | new Avatar(msg.Role == "user" ? "You" : "AI")
                                          .Width(30).Height(30)
                                    | new Card(
                                        Text.Literal(msg.Content).Width(Size.Grow())
                                      )
                                )
                        )
                        //| (isLoading.Value
                        //    ? Icons.LoaderCircle
                        //        .ToIcon()
                        //        .Color(Colors.Gray)
                        //        .WithAnimation(AnimationType.Rotate)
                        //    : null)
                )

                // Input Section
                | new Card(Layout.Horizontal().Gap(2)
                    | new TextInput(userInput)
                              .Placeholder("Type your message...")
                              .Disabled(isLoading.Value)
                    | new Button("Send", onClick: async _ =>
                        await SendMessage(userInput, conversation, isLoading))
                              .Icon(Icons.Send)
                              .Disabled(isLoading.Value || string.IsNullOrEmpty(userInput.Value))
                );
        }

        private async ValueTask SendMessage(
            IState<string> userInput,
            IState<List<ChatMessage>> conversation,
            IState<bool> isLoading)
        {
            if (string.IsNullOrEmpty(userInput.Value)) return;

            isLoading.Set(true);

            try
            {
                // Add user message
                var userMessage = new ChatMessage { Role = "user", Content = userInput.Value };
                var updatedConversation = conversation.Value.ToList();
                updatedConversation.Add(userMessage);
                conversation.Set(updatedConversation);

                var currentInput = userInput.Value;
                userInput.Set(""); // clear

                // Initialize new ChatClient
                var client = new ChatClient(Model, ApiKey);

                // Prepare conversation for API
                var messages = updatedConversation
                    .Select<ChatMessage, OpenAI.Chat.ChatMessage>(m =>
                        m.Role == "user"
                            ? new UserChatMessage(m.Content)
                            : new AssistantChatMessage(m.Content)
                    )
                    .ToList();

                // Get response
                ChatCompletion completion = await client.CompleteChatAsync(messages);
                string aiResponse = completion.Content[0].Text;

                // Add assistant message
                updatedConversation.Add(new ChatMessage { Role = "assistant", Content = aiResponse });
                conversation.Set(updatedConversation);
            }
            catch (Exception ex)
            {
                var errorMessage = new ChatMessage
                {
                    Role = "assistant",
                    Content = $"Error: {ex.Message}"
                };
                var updatedConversation = conversation.Value.ToList();
                updatedConversation.Add(errorMessage);
                conversation.Set(updatedConversation);

                this.UseService<IClientProvider>().Toast("Error communicating with OpenAI");
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
