using OfficeOpenXml;
using SalesManBotWithEmbeddings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManBotWithEmbeddings
{
    public class ExcelTrainingPhraseReader
    {
        private readonly EmbeddingsProvider _embeddingsProvider;

        public ExcelTrainingPhraseReader(EmbeddingsProvider embeddingsProvider)
        {
            _embeddingsProvider = embeddingsProvider;
        }

        public async Task<List<TrainingPhrase>> GetTrainingPhrases()
        {

            List<TrainingPhrase> result = new List<TrainingPhrase>();

            FileInfo existingFile = new FileInfo("../../../SourcePromps/TrainingPhrases.xlsx");
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count
                for (int row = 2; row <= rowCount; row++)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        string value = worksheet.Cells[row, col].Value?.ToString().Trim();

                        List<double> embeddings = await _embeddingsProvider.GetEmbeddings(value);

                        result.Add(new TrainingPhrase(row.ToString(), value, embeddings));
                    }
                }
            }


            return result;
        }

    }
}
