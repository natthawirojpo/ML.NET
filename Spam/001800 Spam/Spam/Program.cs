//  * LOY 2019 ML.NET Course
using Common;
using Microsoft.ML;
using System;
using System.Linq;

namespace Spam
{
    class Program
    {
        private static string trainDataPath = @"E:\ML\spam-train.tsv";
        private static string saveModelPath = @"E:\ML\spam-model.zip";

        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();

            // Specify the schema for spam data and read it into DataView.
            var trainDataView = mlContext.Data.LoadFromTextFile<SpamInput>
                (path: trainDataPath, hasHeader: true, separatorChar: '\t');


            // Create the estimator which converts the text label to boolean
            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")

                // featurizes column "Message" to a new column "FeaturesText"
                .Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", 
                    new Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.Options
                    {
                        WordFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
                            { NgramLength = 2, UseAllLengths = true },
                        CharFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
                            { NgramLength = 3, UseAllLengths = false },
                    }, "Message"))

                // copy column "FeaturesText" to "Features"
                .Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText"))

                // nomalize column "Features"
                .Append(mlContext.Transforms.NormalizeLpNorm("Features", "Features"))

                // It is helpful to have a caching checkpoint before trainers 
                // that take multiple data passes.
                .AppendCacheCheckpoint(mlContext);


            // Set the training algorithm  **************************
            var trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll
                (mlContext.BinaryClassification.Trainers.AveragedPerceptron
                (labelColumnName: "Label", 
                numberOfIterations: 10, 
                featureColumnName: "Features"), 
                labelColumnName: "Label")

                // convert Label's key back to value
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

            // create traning pipeline
            var trainingPipeLine = dataProcessPipeline.Append(trainer);


            // Evaluate *******************
            // Evaluate the model using cross-validation.
            // Cross-validation splits our dataset into 'folds', trains a model on some folds and 
            // evaluates it on the remaining fold. We are using 5 folds so we get back 5 sets of scores.
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.MulticlassClassification.CrossValidate
                (data: trainDataView, 
                estimator: trainingPipeLine, 
                numberOfFolds: 5);

            // Show validation score
            // Let's compute the average AUC, which should be between 0.5 and 1 (higher is better).
            ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics
                (trainer.ToString(), crossValidationResults);



            // Train ***************
            // Now let's train a model on the full dataset to help us get better results
            var model = trainingPipeLine.Fit(trainDataView);



            //Create a PredictionFunction from our model 
            var predictor = mlContext.Model.CreatePredictionEngine<SpamInput, SpamPrediction>(model);

            Console.WriteLine("=============== Predictions for below data===============");
            // Test a few examples
            ClassifyMessage(predictor, "That's a great idea. It should work.");
            ClassifyMessage(predictor, "free medicine winner! congratulations");
            ClassifyMessage(predictor, "Yes we should meet over the weekend!");
            ClassifyMessage(predictor, "you win pills and free entry vouchers");

            Console.WriteLine("=============== End of process, hit any key to finish =============== ");
            Console.ReadLine();

            // save ML model for production use
            mlContext.Model.Save(model, trainDataView.Schema, saveModelPath);
        }

        public static void ClassifyMessage(PredictionEngine<SpamInput, SpamPrediction> predictor, string message)
        {
            var input = new SpamInput { Message = message };
            var prediction = predictor.Predict(input);

            Console.WriteLine("The message '{0}' is {1}", 
                input.Message, 
                prediction.isSpam == "spam" ? "spam" : "not spam");
        }
    }
}
