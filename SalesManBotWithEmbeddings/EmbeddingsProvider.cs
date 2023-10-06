using OpenAI.Managers;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;

namespace SalesManBotWithEmbeddings
{
    public class EmbeddingsProvider
    {
        private readonly IOpenAIService _openAIService;
        public EmbeddingsProvider(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<List<double>> GetEmbeddings(string text)
        {
            OpenAI.ObjectModels.ResponseModels.EmbeddingCreateResponse embeddingResult = await _openAIService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest()
            {
                InputAsList = new List<string> { text },
                Model = Models.TextEmbeddingAdaV2
            });

            OpenAI.ObjectModels.ResponseModels.EmbeddingResponse result = embeddingResult.Data.First();

            return result.Embedding;
        } 

    }
}
