using SalesManBotWithEmbeddings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinecone;

namespace SalesManBotWithEmbeddings
{
    internal class TrainingPhraseRepository
    {

        private readonly PineconeClient _pineconeClient;

        private string IndexName =>  "my-index";

        public TrainingPhraseRepository(PineconeClient pineconeClient)
        {
            _pineconeClient = pineconeClient;
        }


        public async Task CreateIndex()
        {
            // List all indexes
            var indexes = await _pineconeClient.ListIndexes();

            // Create a new index if it doesn't exist
            var indexName = IndexName;
            if (indexes.Contains(indexName))
                await _pineconeClient.DeleteIndex(indexName);

            await _pineconeClient.CreateIndex(indexName, 1536, Metric.Cosine);
        }


        public async Task Create(List<TrainingPhrase> trainingPhrases)
        {
            // Get an index by name
            using var index = await _pineconeClient.GetIndex(IndexName);


            Vector[] vectors = trainingPhrases.Select(promp => new Vector
            {
                Id = promp.Id,
                Values = promp.Embeddings.Select(e => (float)e).ToArray(),
                Metadata = new MetadataMap
                {
                    ["Content"] = promp.Content
                }
            }).ToArray();

            await index.Upsert(vectors);
        }


        public async Task<List<TrainingPhrase>> Query(List<double> embeddings, uint top)
        {
            // Get an index by name
            using var index = await _pineconeClient.GetIndex(IndexName);

            float[] vector = embeddings.Select(x => (float)x).ToArray();

            ScoredVector[] scored = await index.Query(vector, top);
            //todo aqui no se como convertir al model
            return scored.Select(o => new TrainingPhrase(o.Id, null, null)).ToList();
        }


    }
}
