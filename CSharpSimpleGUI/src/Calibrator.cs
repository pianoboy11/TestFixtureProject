using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDriverCSharpDemo
{
    // SPAM may well have better implementations of any or all of these, but I haven't
    // dug through the docs to find them.
    class Calibrator
    {
        Calibrator()
        {
        }

        // given an array, an index within the array, and a "boxcar half size",
        // find the difference between the highest and lowest points within
        // that boxcar
        static private double findMaxDelta(ref double[] values, int pos, int size)
        {
            int start = pos - size;
            int end = pos + size;
            if (start < 0)
                start = 0;
            if (end >= values.Length)
                end = values.Length - 1;
            double min = values[start];
            double max = values[start];
            for (int i = start + 1; i <= end; i++)
            {
                if (values[i] < min)
                    min = values[i];
                if (values[i] > max)
                    max = values[i];
            }
            return max - min;
        }

        static public double getBaseline(ref double[] values)
        {
            List<double> counts = new List<double>();
            for (int i = 0; i < values.Length; i++)
                counts.Add(values[i]);

            // sort the deltas
            counts.Sort();
            
            // drop high and low 10%
            int trim = (int) (counts.Count / 10);
            counts.RemoveRange(0, trim);
            counts.RemoveRange(counts.Count - trim - 1, trim);

            // take average of remainder
            double sum = 0;
            for (int i = 0; i < counts.Count; i++)
                sum += counts[i];

            return sum / counts.Count;
        }

        static public double getAverageNoise(ref double[] values)
        {
            // compute boxcar deltas across the spectrum
            List<double> deltas = new List<double>();
            for (int i = 0; i < values.Length; i++)
            {
                double maxDelta = findMaxDelta(ref values, i, 5);
                deltas.Add(maxDelta);
            }

            // sort the deltas
            deltas.Sort();
            
            // take the midpoint
            return deltas[(int)(deltas.Count / 2)];
        }
    }
}
