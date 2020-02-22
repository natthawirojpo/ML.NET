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

            // Inspect IDataView values one row at a time
            IEnumerable<byte> sizeColumn = data.GetColumn<byte>("Age").ToList();

            foreach (var v in sizeColumn)
            {
                Console.Write($"{v} , ");
            }
        }
    }
}
