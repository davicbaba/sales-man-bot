﻿using OpenAI.Managers;
using OpenAI;
using Pinecone;
using OpenAI.Interfaces;
using SalesManBotWithEmbeddings;
using System.Reflection;
using OfficeOpenXml;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Microsoft.Extensions.Configuration;

var configurationBuilder = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");
IConfigurationRoot configuration = configurationBuilder.Build();

string? openAiApiKey = configuration["OpenAIConfig:ApiKey"];
string? pineconApiKey = configuration["PineconConfig:ApiKey"];
string? pineconEnvironment = configuration["PineconConfig:Environment"];

ArgumentException.ThrowIfNullOrEmpty(openAiApiKey);
ArgumentException.ThrowIfNullOrEmpty(pineconApiKey);
ArgumentException.ThrowIfNullOrEmpty(pineconEnvironment);

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = openAiApiKey
});

using var pinecone = new PineconeClient(pineconApiKey, pineconEnvironment);

TrainingPhraseRepository repo = new TrainingPhraseRepository(pinecone);

EmbeddingsProvider embeddingsProvider = new EmbeddingsProvider(openAiService);

ExcelTrainingPhraseReader excelReader = new ExcelTrainingPhraseReader(embeddingsProvider);


Console.WriteLine("Creating Index....");

await repo.CreateIndex();

Console.WriteLine("Getting data from excel....");

List<SalesManBotWithEmbeddings.Model.TrainingPhrase> trainingPhrases = await excelReader.GetTrainingPhrases();

Console.WriteLine("Uploading embeddings");

await repo.Create(trainingPhrases);


while (true)
{
    Console.WriteLine("Write a promp.");

    string? userPromp = Console.ReadLine();

    if (string.IsNullOrEmpty(userPromp))
    {
        Console.WriteLine("Please Write a message");
        continue;
    }

    var embedding = await embeddingsProvider.GetEmbeddings(userPromp);

    List<SalesManBotWithEmbeddings.Model.TrainingPhrase> topPhrases = await repo.Query(embedding, top: 5);

    foreach(var phrase in topPhrases)
    {
        Console.WriteLine($"Id: {phrase.Id}. Content: {phrase.Content}");
    }


}






