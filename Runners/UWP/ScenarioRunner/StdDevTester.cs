using System;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioRunner
{
    public static class StdDevTester
    {
        private static void StardardDev()
        {
            Random rand = new Random();
            double mean = 0;
            double stdDev = 0.2;

            Dictionary<double, int> rsn = new Dictionary<double, int>();
            Dictionary<double, int> rn = new Dictionary<double, int>();
            Dictionary<double, int> some = new Dictionary<double, int>();

            for(int i = 0; i < 10000; i++)
            {
                double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                                       * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                IncrementDictionary(Math.Round(randStdNormal, 2), rsn);
                double randNormal = mean + stdDev * randStdNormal;      //random normal(mean,stdDev^2)
                IncrementDictionary(Math.Round(randNormal, 2), rn);

                double val = rand.NextDouble() + rand.NextDouble() - 1.0;
                IncrementDictionary(Math.Round(val, 2), some);
            }

            List<double> sorted = rsn.Keys.ToList();
            sorted.Sort();
            sorted.ForEach((d) => Console.WriteLine(d + "|" + rsn[d]));

            Console.WriteLine("==========================");

            sorted = rn.Keys.ToList();
            sorted.Sort();
            sorted.ForEach((d) => Console.WriteLine(d + "|" + rn[d]));

            Console.WriteLine("==========================");

            sorted = some.Keys.ToList();
            sorted.Sort();
            sorted.ForEach((d) => Console.WriteLine(d + "|" + some[d]));
        }
        private static void IncrementDictionary(double theValue, Dictionary<double, int> stuff)
        {
            if(!stuff.ContainsKey(theValue))
            {
                stuff.Add(theValue, 0);
            }
            stuff[theValue]++;
        }
    }
}
