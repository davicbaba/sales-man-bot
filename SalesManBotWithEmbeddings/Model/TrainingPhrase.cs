using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManBotWithEmbeddings.Model
{
    public class TrainingPhrase
    {
        public TrainingPhrase(string id, string content, List<double> embeddings)
        {
            Id = id;
            Content = content;
            Embeddings = embeddings;
        }

        public string Id { get;  }


        public string Content { get; }

        public List<double> Embeddings { get; }


    }
}
