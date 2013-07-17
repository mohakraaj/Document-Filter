using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace DocumentFilter
{
    class Classifier
    {
        public SqlCeConnection connection;
        public Dictionary<String,Dictionary<String, int>> featureCatCount = new Dictionary<String,Dictionary<string, int>>();
        public  Dictionary<String, int> categoryCount = new Dictionary<string, int>();
        public  Func<String, Dictionary<String, int>> getfeatures;

        public Classifier(Func<String, Dictionary<String, int>> getfeatures)
        {
            this.getfeatures = getfeatures;
            // set the connection path specific to your database location
            connection = new SqlCeConnection("Data Source=C:\\Users\\Rajendra.lenovo-PC\\Documents\\DocumentFilter.sdf");
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        // close database connnections 
       ~Classifier()
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // create tables
        public void createdatabase()
        {
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            try
            {
                command.CommandText = "drop table featureCount;";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("drop table problem" + e.ToString());
            }
            try
            {
                command.CommandText = "drop table categoryCount;";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("drop table problem" + e.ToString());
            }

            try
            {
                command.CommandText = "CREATE TABLE featureCount (feature nvarchar(200),category nvarchar(200), count int, PRIMARY KEY(feature,category) ); ";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("feature count problem" + e.ToString());
            }
            try
            {
                command.CommandText = "CREATE TABLE categoryCount (category nvarchar(200) PRIMARY KEY, count int); ";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("category count problem" + e.ToString());
            }
         
        }

        // Increase the count of a feature/category pair
        public void incrementFeatureCount(String feature, String cat)
        {
            // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
         /*   if (!featureCatCount.ContainsKey(feature))
                featureCatCount.Add(feature,new Dictionary<string,int>());
            if(!featureCatCount[feature].ContainsKey(cat))
                featureCatCount[feature].Add(cat,0);
            featureCatCount[feature][cat]++; */
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            int count = getFeatureCount(feature, cat);
            if (count == 0)
            {
                try
                {
                    command.CommandText = "insert into featureCount(feature,category,count) values (\'" + feature + "\',\'" + cat + "\'," + 1 + ");";
                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert Feature count " + e.ToString());
                }
            }
            else
            {
                try
                {
                    command.CommandText = "update featureCount set count="+(count+1)+" where feature=\'"+feature+"\' and category=\'"+cat+"\';";
  
                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("update Feature count " + e.ToString());
                }


            }

        }

        // Increase the count of a category
        public void incrementCategoryCount(String cat)
        {
            // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
          /*  if (!categoryCount.ContainsKey(cat))
                categoryCount.Add(cat, 0);
            categoryCount[cat]++; */
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            int count =getCategoryCount(cat);
            if (count == 0)
            {
                try
                {
                    command.CommandText = "insert into categoryCount(category,count) values (\'" + cat + "\'," + 1 + ");";
                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert Category count ::" + e.ToString());
                }
            }
            else
            {
                try
                {
                    command.CommandText = "update categoryCount set count=" + (count + 1) + " where  category=\'" + cat + "\';";

                    command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("update Category count " + e.ToString());
                }


            }

        }

        // The number of times a feature has appeared in a category
        public int getFeatureCount(String feature,String cat)
        {
            // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
            /*
            if (!featureCatCount.ContainsKey(feature) || !featureCatCount[feature].ContainsKey(cat))
                return 0;
            return featureCatCount[feature][cat];
             */
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            SqlCeDataReader myReader = null;
            try
            {
                command.CommandText = "Select count from featureCount where feature=\'"+feature+"\' and category=\'"+cat+"\';";
                myReader = command.ExecuteReader();
                if (myReader.Read())
                {
                    return int.Parse(myReader["count"].ToString());
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("getfeaturecount error " + e.ToString());
                return 0;
            }

        }

        //The number of items in a category 
        public int getCategoryCount(String cat)
        { // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
          /*  if (!categoryCount.ContainsKey(cat))
                return 0;
            return categoryCount[cat]; */
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            SqlCeDataReader myReader = null;
            try
            {
                command.CommandText = "Select count from categoryCount where category=\'" + cat + "\';";
                myReader = command.ExecuteReader();
                if (myReader.Read())
                {
                    return int.Parse(myReader["count"].ToString());
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("getcategory count error "  + e.ToString());
                return 0;
            }

        }

        // The total number of items
        public int getItemsCount()
        {
            // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
             //return categoryCount.Values.Sum();
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            SqlCeDataReader myReader = null;
            try
            {
                command.CommandText = "Select sum(count) from categoryCount ;";
                myReader = command.ExecuteReader();
                if (myReader.Read())
                {
                    return int.Parse(myReader.GetValue(0).ToString());
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("total count error " + e.ToString());
                return 0;
            }

        }

        //The list of all categories 
        public List<String> getCategories()
        {
            // uncomment the commented line and comment the uncommented line if yuo do not intend to use the database
            SqlCeCommand command = new SqlCeCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            List<String> categories = new List<String>();
            SqlCeDataReader myReader = null;
            try
            {
                command.CommandText = "Select category from categoryCount ;";
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    categories.Add(myReader["category"].ToString());
                }
               
                return categories;
            }
            catch (Exception e)
            {
                Console.WriteLine("total count error " + e.ToString());
                return null;
            }

          //  return categoryCount.Keys.ToList();
        }

        // train your users   
        public void train(String item, String cat)
        {
            var features = getfeatures(item);
            foreach(var word in features.Keys)
            incrementFeatureCount(word, cat);
            incrementCategoryCount(cat);
        }

        // find the probability
        public double findProbability(String item, String cat)
        {
            //The total number of times this feature appeared in this
            // category divided by the total number of items in this category
            if (getCategoryCount(cat) == 0)
            {
                Console.WriteLine("check");
                return 0;
            }
            return ((double)getFeatureCount(item, cat)) /(double) (getCategoryCount(cat));
        }

        public double weightedProbabilty(String item, String cat,Func<String,String,double> fprob, double assumed_probability = 0.5, double weight = 1 )
        {
            var prob = fprob(item, cat);
            var total = from cate in getCategories() select getFeatureCount(item, cate);
            return (assumed_probability*weight+prob*total.Sum())/(total.Sum()+ weight);
        }

        public void sampletrain()
        {
            train("Nobody owns the water.", "good");
            train("the quick rabbit jumps fences", "good");
            train("buy pharmaceuticals now", "bad");
            train("make quick money in the online casino", "bad");
            train("the quick brown fox jumps", "good");

        }
    }
}
