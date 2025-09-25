using System.Text;
using OllamaSharp;
using Chat = Ivy.Chat;
using ChatMessage = Ivy.ChatMessage;

namespace OllamaSharpDemo.Apps;

[App(icon: Icons.TextQuote, title: "OllamaSharp")]
public class OllamaSharpSample : ViewBase
{
    private const string Url = "http://localhost:11434";
    private IState<ImmutableArray<ChatMessage>> messages;
    private IState<ImmutableArray<ModelListItem>> models;
    private IState<ModelListItem?> selectedModel;
    private OllamaApiClient ollamaApiClient;

    public override object? Build()
    {
        messages = UseState(ImmutableArray.Create<ChatMessage>());

        models = UseState(ImmutableArray.Create<ModelListItem>);
        selectedModel = UseState<ModelListItem?>();
        
        if (selectedModel.Value == null)
            return Layout.Center()
                   | (new Card(
                           Layout.Vertical().Gap(6).Padding(2)
                           | new Button("Refresh Models", onClick: _ => OnRefreshClicked())
                           | Layout.Vertical(models.Value)
                       )
                   )
                   .Width(Size.Units(120).Max(500));

        return Layout.Vertical().Padding(0, 10, 0, 10)
               | Text.H4($"Chatting with model: {selectedModel.Value?.ModelName}")
               | new Button("Back to Models", onClick: _ =>
               {
                   ClearModelSelection();
               })
               | new Chat(messages.Value.ToArray(), OnSendMessage).Width(Size.Full().Max(200));
    }

    private async ValueTask OnSendMessage(Event<Chat, string> @event)
    {
        messages.Set(messages.Value.Add(new ChatMessage(ChatSender.User, @event.Value)));
        ollamaApiClient.SelectedModel = selectedModel.Value.ModelName;
        var chat = new OllamaSharp.Chat(ollamaApiClient, @event.Value);
        var builder = new StringBuilder();
        await foreach (var answerToken in chat.SendAsync(@event.Value))
        {
            builder.Append(answerToken);
        }

        messages.Set(messages.Value.Add(new ChatMessage(ChatSender.Assistant, builder.ToString())));
    }

    private async ValueTask OnRefreshClicked()
    {
        ollamaApiClient?.Dispose();
        ollamaApiClient = new OllamaApiClient(Url);
        var connected = await ollamaApiClient.IsRunningAsync();
        if (!connected)
        {
            var client = UseService<IClientProvider>();
            client.Toast($"Ollama API is not running at {Url}", "Connection Error");
            return;
        }

        var ollamaModels = await ollamaApiClient.ListLocalModelsAsync();
        models.Set(ollamaModels.Select(m => new ModelListItem(s => { selectedModel.Set(s); }, m.Name))
            .ToImmutableArray());
        ClearModelSelection();
    }

    private void ClearModelSelection()
    {
        selectedModel.Set((ModelListItem?)null);
        messages.Set([]);
    }
}

public class ModelListItem(Action<ModelListItem> modelSelect, string modelName) : ViewBase
{
    public string ModelName { get; } = modelName;

    public override object? Build()
    {
        return Layout.Vertical(
            Layout.Horizontal(
                new Button(ModelName, _ => { modelSelect(this); }).Icon(Icons.NotebookText)
                    .Variant(ButtonVariant.Outline)
            ).Align(Align.Left).Width(Size.Units(120).Max(500)),
            new Separator()
        );
    }
}