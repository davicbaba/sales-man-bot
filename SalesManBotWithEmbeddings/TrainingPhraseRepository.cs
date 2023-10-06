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
                return;

            await _pineconeClient.CreateIndex(indexName, 1024, Metric.Cosine);

        }


        public async Task Create(List<TrainingPhrase> trainingPhrases)
        {
            // Get an index by name
            var index = await _pineconeClient.GetIndex(IndexName);

            while(index.Status.IsReady == false)
            {
                Console.WriteLine("Index is not ready.... Waiting 5 seconds to retry");

                await Task.Delay(5000);

                index = await _pineconeClient.GetIndex(IndexName);
            }

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

            ScoredVector[] scored = await index.Query(vector, top, includeMetadata: true);

            //todo aqui no se como convertir al model
            return scored.Select(o => {

                string content = o.Metadata["Content"].Inner.ToString();
                List<double> embeddings = o.Values.Select(t => (double)t).ToList();

                return new TrainingPhrase(o.Id, content, embeddings);
            }).ToList();
        }


    }
}
