﻿//  * LOY 2019 ML.NET Course
using System;
using Microsoft.ML;
using System.IO;
using System.Collections.Generic;

namespace Sales_spike
{
    class Program
    {
        private static string DatasetPath = @"E:\ML\product-sales.csv";
        private static string ModelPath = @"E:\ML\ProductSalesModel.zip";
        private static MLContext mlContext;

        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            mlContext = new MLContext();

            //assign the Number of records in dataset file to cosntant variable
            const int size = 36;

            //Load the data into IDataView.
            //This dataset is used while prediction/detecting spikes or changes.
            IDataView dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>
                (path: DatasetPath, hasHeader: true, separatorChar: ',');

            //To detech temporay changes in the pattern
            DetectSpike(size, dataView);

            // To detech changepoint
            DetectChangepoint(size, dataView);
        }

        static void DetectSpike(int size, IDataView dataView)
        {
            //STEP 1: Create Esimtator   
            // The confidence for spike detection in the range [0, 100].
            // The size of the sliding window for computing the p - value.
            var estimator = mlContext.Transforms.DetectIidSpike
                (outputColumnName: nameof(ProductSalesPrediction.Prediction), 
                inputColumnName: nameof(ProductSalesData.numSales), 
                confidence: 95, 
                pvalueHistoryLength: size / 4); 

            //STEP 2:The Transformed Model.
            //In IID Spike detection, we don't need to do training, we just need to do transformation. 
            //As you are not training the model, there is no need to load IDataView with real data, 
            //you just need schema of data.
            //So create empty data view and pass to Fit() method. 
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            //STEP 3: Use/test model
            //Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>
                (transformedData, reuseRowObject: false);

            // P-values are probability values
            Console.WriteLine("Alert\tScore\tP-Value");
            foreach (var p in predictions)
            {
                if (p.Prediction[0] == 1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2]);
                Console.ResetColor();
            }
            Console.WriteLine("");
        }

        static void DetectChangepoint(int size, IDataView dataView)
        {
            Console.WriteLine("===============Detect Persistent changes in pattern===============");

            //STEP 1: Setup transformations using DetectIidChangePoint
            var estimator = mlContext.Transforms.DetectIidChangePoint
                (outputColumnName: nameof(ProductSalesPrediction.Prediction), 
                inputColumnName: nameof(ProductSalesData.numSales), 
                confidence: 95, 
                changeHistoryLength: size / 4);

            //STEP 2:The Transformed Model.
            //In IID Change point detection, we don't need need to do training, we just need to do transformation. 
            //As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            //So create empty data view and pass to Fit() method. 
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            //STEP 3: Use/test model
            //Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>
                (transformedData, reuseRowObject: false);

            // martingales value are a function of the p-values generated from a set of data.
            Console.WriteLine($"{nameof(ProductSalesPrediction.Prediction)} column obtained post-transformation.");
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");

            foreach (var p in predictions)
            {
                if (p.Prediction[0] == 1)
                {
                    Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}\t{3:0.00}  <-- alert is on, predicted changepoint", p.Prediction[0], p.Prediction[1], p.Prediction[2], p.Prediction[3]);
                }
                else
                {
                    Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}\t{3:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2], p.Prediction[3]);
                }
            }
            Console.WriteLine("");
        }

        private static IDataView CreateEmptyDataView()
        {
            //Create empty DataView. We just need the schema to call fit()
            IEnumerable<ProductSalesData> enumerableData = new List<ProductSalesData>();
            var dv = mlContext.Data.LoadFromEnumerable(enumerableData);
            return dv;
        }

    }
}


