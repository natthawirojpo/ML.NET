using System;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Linq;
using System.IO;
using static Microsoft.ML.DataOperationsCatalog;
using System.IO.Compression;

namespace Credit
{
    class Program
    {
        static void Main(string[] args)
        {
            string zipDataSet = @"E:\ml\creditcardfraud.zip";
            string extracPath = @"E:\ml\";
            string fullDataSetFilePath = @"E:\ml\creditcard.csv";
            string trainDataSetFilePath = @"E:\ml\trainData.csv";
            string testDataSetFilePath = @"E:\ml\testData.csv";
            //string modelFilePath = Path.Combine(assetsPath, "output", "fastTree.zip");

            MLContext mlContext = new MLContext(seed: 1);

            ZipFile.ExtractToDirectory(zipDataSet, extracPath);

            // Prepare data and create Train/Test split datasets
            PrepDatasets(mlContext, fullDataSetFilePath, trainDataSetFilePath, testDataSetFilePath);
        }
        public static void PrepDatasets(MLContext mlContext, 
            string fullDataSetFilePath, 
            string trainDataSetFilePath, 
            string testDataSetFilePath)
        {
            //Only prep-datasets if train and test datasets don't exist yet

            if (!File.Exists(trainDataSetFilePath) &&
                !File.Exists(testDataSetFilePath))
            {
                Console.WriteLine("===== Preparing train/test datasets =====");


                //Load the original single dataset
                IDataView originalFullData = mlContext.Data.LoadFromTextFile<TransactionObservation>
                    (fullDataSetFilePath, 
                    separatorChar: ',', 
                    hasHeader: true);

                // Split the data 80:20 into train and test sets, train and evaluate.
                TrainTestData trainTestData = mlContext.Data.TrainTestSplit(originalFullData, testFraction: 0.2, seed: 1);
                IDataView trainData = trainTestData.TrainSet;
                IDataView testData = trainTestData.TestSet;

                //Inspect TestDataView to make sure there are true and false observations in test dataset, after spliting 
                InspectData(mlContext, testData, 4);

                // save train split
                using (var fileStream = File.Create(trainDataSetFilePath))
                {
                    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar: ',', headerRow: true, schema: true);
                }

                // save test split 
                using (var fileStream = File.Create(testDataSetFilePath))
                {
                    mlContext.Data.SaveAsText(testData, fileStream, separatorChar: ',', headerRow: true, schema: true);
                }
            }
        }
        public static void InspectData(MLContext mlContext, IDataView data, int records)
        {
            //We want to make sure we have True and False observations
            Console.WriteLine("Show 4 fraud transactions (true)");

            ShowObservationsFilteredByLabel(mlContext, data, label: true, count: records);

            Console.WriteLine("Show 4 NOT-fraud transactions (false)");
            ShowObservationsFilteredByLabel(mlContext, data, label: false, count: records);
        }
        public static void ShowObservationsFilteredByLabel(MLContext mlContext, IDataView dataView, bool label = true, int count = 2)
        {
            // Convert to an enumerable of user-defined type. 
            var data = mlContext.Data.CreateEnumerable<TransactionObservation>(dataView, reuseRowObject: false)
                                            .Where(x => x.Label == label)
                                            // Take a couple values as an array.
                                            .Take(count)
                                            .ToList();

            // print to console
            data.ForEach(row => { row.PrintToConsole(); });
        }
    }
}
