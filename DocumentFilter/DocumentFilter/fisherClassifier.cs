using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentFilter
{
    class fisherClassifier : Classifier
    {
        Dictionary<String, double> minimum = new Dictionary<string, double>();
        public fisherClassifier(Func<String, Dictionary<String, int>> getfeatures) : base(getfeatures) { }

        public double fprob(String feature, String cat)
        {
            var prob = findProbability(feature, cat);
            if (prob == 0)
                return 0;
            var frequency = from cate in getCategories() select findProbability(feature, cate);
            return (prob) / (double)(frequency.Sum());
        }

        public double fisherprobabilty(String text, String cat)
        {
            var features = getfeatures(text);
            double prob = 1;
            foreach (var feature in features.Keys)
            {
                prob *= weightedProbabilty(feature, cat, fprob);
            }
            prob = -2*Math.Log(prob);
            return chisqare(prob, 2* features.Count);

        }

        private double chisqare(double chi, double df)
        {
            double sum, term;
            term = Math.Exp(-(chi / 2));
            sum = Math.Exp(-(chi / 2));
            for (int i = 1; i < df / 2; i++)
            {
                term *= (chi / 2) / i;
                sum += term;
            }
            return Math.Min(sum, 1.0);
        }

        public void setMinimum(String cat, double threshold)
        {
            if (!minimum.ContainsKey(cat))
                minimum.Add(cat, 0);

            minimum[cat] = threshold;
        }
        public double getMinimum(String cat)
        {
            if (!minimum.ContainsKey(cat))
                return 1;
            return minimum[cat];
        }
        public String classify(String item, String defaulte = "NONE")
        {
            double best_val = 0,prob;
            String best = defaulte;
            foreach (var cat in getCategories())
            {
                prob = fisherprobabilty(item, cat);
                if (prob > best_val && prob > getMinimum(cat))
                {
                    best = cat;
                    best_val = prob;
                }
            }
            return best;
        }
    }
}
