// See https://aka.ms/new-console-template for more information

using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using SalesManBot;

string? apiKey = Environment.GetEnvironmentVariable("MY_OPEN_AI_API_KEY");

ArgumentException.ThrowIfNullOrEmpty(apiKey);

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey =  apiKey
});

string menu = Constants.SystemPromp;

List<ChatMessage> fullConversation = new List<ChatMessage>()
{
    ChatMessage.FromSystem(menu),
};

Console.WriteLine("Write a promp to start the chat.");

while (true)
{
    string? userPromp = Console.ReadLine();

    if (string.IsNullOrEmpty(userPromp))
    {
        Console.WriteLine("Please Write a message");
        continue;
    }
    
    string result = await GetNextPromp(openAiService, fullConversation,userPromp);
    
    Console.WriteLine(result);
    
    fullConversation.Add(ChatMessage.FromAssistant(result));
}

async Task<string> GetNextPromp(OpenAIService aiService, 
                                List<ChatMessage> conversation,
                                string userPromp)
{
    List<ChatMessage> conversationCopy = conversation.ToList();
    conversationCopy.Add(ChatMessage.FromUser(userPromp));
    
    var completionResult = await aiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
    {
        Messages = conversationCopy,
        Model = Models.Gpt_3_5_Turbo,
    });
    if (completionResult.Successful == false)
        throw new ArgumentException("Failed to get the next promp");
    
    return completionResult.Choices.First().Message.Content;
}




