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
                @"C:\@Work\MLNET\Prepare-Data\adultSmall.csv",
                separatorChar: ',',
                hasHeader: true);

            Preview(data);
        }

        public static void Preview(IDataView data)
        {
            var myPreview = data.Preview();

            Console.WriteLine("\n-----------show original data-------- ");

            // -------------- show header
            for (int k = 0; k < myPreview.Schema.Count(); k++)
                Console.Write($"{myPreview.Schema[k]} | ");
            Console.WriteLine("\n----------------------------------- ");

            // ------------- show data rows
            for (int j = 0; j < myPreview.RowView.Count(); j++)
            {
                for (int i = 0; i < myPreview.ColumnView.Count(); i++)
                {
                    var v = myPreview.ColumnView[i].Values.GetValue(j);
                    Console.Write($"{v}\t|");
                }
                Console.WriteLine();
            }
        }


    }
}
