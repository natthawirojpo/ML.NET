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

            // ---------- View data in IDataView ------------------
            // Convert IDataView to IEnumerable
            IEnumerable<Adult> myAdult = mlContext.Data.CreateEnumerable<Adult>(
                data, 
                reuseRowObject: true); // reuseRow to optimize performance

            foreach (var v in myAdult)
            {
                Console.WriteLine($"{v.Id}\t{v.Age}\t{v.Workclass}\t{v.Education}");
            }
        }
    }
}
