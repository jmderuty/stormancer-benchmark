using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Benchmark.Worker
{
    class Request
    {
        public long Id { get; set; }

        public Stopwatch Sw = new Stopwatch();

        public DateTime SentOn { get; set; }
    }
}
