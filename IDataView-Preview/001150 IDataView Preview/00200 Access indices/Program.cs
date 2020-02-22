//  * LOY 2019 ML.NET Course

using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Transforms;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create MLContext
            MLContext mlContext = new MLContext();

            //Load Data
            IDataView data = mlContext.Data.LoadFromTextFile<Adult>(
                @"E:\ml\adultSmall.csv",
                separatorChar: ',',
                hasHeader: true);

            const int max = 3;

            // Convert IDataView to IEnumerable
            Adult[] myAdult = mlContext.Data.CreateEnumerable<Adult>(
                data, 
                reuseRowObject: false)
                .Take(max)
                .ToArray();

            // Accessing specific indices with IEnumerable
            for (int i = 0; i < max; i++)
            {
                Console.WriteLine(myAdult[i].Workclass);
            }
        }
    }
}
