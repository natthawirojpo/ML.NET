//  * LOY 2019 ML.NET Course
using Microsoft.ML.Data;

using System;
using System.Collections.Generic;
using System.Text;

namespace test
{
    public partial class Adult
    {
        [LoadColumn(0)]
        public short Id { get; set; }

        [LoadColumn(1)]
        public byte Age { get; set; }

        [LoadColumn(2)]
        public string Workclass { get; set; }

        [LoadColumn(3)]
        public string Education { get; set; }
    }
}
