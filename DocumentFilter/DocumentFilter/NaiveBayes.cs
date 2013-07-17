using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentFilter
{
    class NaiveBayes : Classifier
    {
        Dictionary<String, double> threshold = new Dictionary<string, double>();

        public NaiveBayes(Func<String, Dictionary<String, int>> getfeatures):base(getfeatures){}

        public double docProbability(String Text, String cat )
        {
            var features = getfeatures(Text);
            double prob = 1.0;
            foreach (var feature in features.Keys)
            {
                prob *= weightedProbabilty(feature, cat,findProbability);

            }
            return prob;
        }

        public double probability(String Text, String cat)
        {
            var pcater = (double) getCategoryCount(cat) / (double)getItemsCount();
            return pcater * docProbability(Text, cat);
        }

        public void setThreshold(String cat, double threshold)
        {
            if (!this.threshold.ContainsKey(cat))
                this.threshold.Add(cat,0);

            this.threshold[cat] = threshold;
        }

        public double getThreshold(String cat)
        {
            if (!threshold.ContainsKey(cat))
                return 1;
            return threshold[cat];
        }

        public String classify(String doc, String defaulte = "None")
        {
            double prob = 0;
            String best=null;
            double best_value=0;
            foreach (var cat in getCategories())
            {
                prob = probability(doc, cat);
                if (prob > best_value)
                {
                    best_value = prob;
                    best = cat;
                }
            }
            foreach (var cat in getCategories())
            {
                if (cat == best) continue;
                prob = probability(doc, cat);
                if (best_value < (getThreshold(best)*prob))
                {
                    return defaulte;
                }
                
            }

            return best;
        }
        
    }
}
