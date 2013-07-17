using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 

namespace DocumentFilter
{
    class Program
    {

        static void Main(string[] args)
        {
            NaiveBayes filter = new NaiveBayes(getWords);
            filter.createdatabase();
            filter.sampletrain();
            Console.WriteLine(filter.classify("quick money"));

            Console.Read();
        }

        static Dictionary<String, int> getWords(String doc)
        {

            Dictionary<String, int> wordlist = new Dictionary<String, int>();
          
            doc = doc.ToLower();
            var words = Regex.Split(doc, @"\W+");
            var allwords = from word in words where word.Length > 2 && word.Length < 20 select word;

            foreach (var word in words)
            {
                if (!wordlist.ContainsKey(word))
                    wordlist.Add(word, 1);
            }
            
            return wordlist;
        }

    }
}
