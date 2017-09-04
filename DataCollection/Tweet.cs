using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using DataCollection.Stem;
using Npgsql;
using NpgsqlTypes;

namespace DataCollection
{
    public class Tweet
    {
        /* for date format (Australia)*/
        private static IFormatProvider culture = new System.Globalization.CultureInfo("en-AU", true);
        
        public String origin { get; set; }
        public DateTime createTime { get; set; }
        public String[] tokenizedTerms { get; set; }

        public Tweet()
        {
        }

        public static Dictionary<String, int> GetMostCommonWords(int suburbId, string date, int hour, int numToTake)
        {
            String terms = GetTerms(suburbId, date, hour);
            return RefineTerms(terms, numToTake);
        }
        
        private static String GetTerms(int suburbId, string date, int hour)
        {
            DataTable dt = new DataTable();
            String Sql = "SELECT suburb_id, string_agg(LTRIM(RTRIM(tokenized,']'), '['),',') AS tokenized_tweepy FROM tweepy WHERE extract(hour from createtime) = :hour and createtime::date = :date and suburb_id = :suburbId and tokenized <> '[]' GROUP BY suburb_id;";

            try
            {
                NpgsqlConnection conn = DBConnect.OpenConnection();
                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = Sql;
                cmd.Parameters.Add(new NpgsqlParameter("hour", NpgsqlDbType.Integer));
                cmd.Parameters.Add(new NpgsqlParameter("date", NpgsqlDbType.Date));
                cmd.Parameters.Add(new NpgsqlParameter("suburbId", NpgsqlDbType.Integer));
                cmd.Prepare();
                
                cmd.Parameters[0].Value = hour;
                cmd.Parameters[1].Value = date;
                cmd.Parameters[2].Value = suburbId;  
                
                NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
                nda.Fill(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when get Terms at SQL query");
                throw e;
            }
            finally
            {
                DBConnect.CloseConnection();
            }

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][1].ToString();
            }
            else
            {
                return null;
            }
        }
        
        private static Dictionary<String, int> RefineTerms(String terms, int NumOfTermsToTake)
        {
            if (!String.IsNullOrEmpty(terms))
            {
                terms = terms.Replace("'", "").Replace("#", "");
                List<String> termList = terms.Split(',').ToList();

                List<String> retList = new List<string>();

                foreach (string term in termList)
                {
                    String refinedTerm = new EnglishStemmer().Stem(term.Trim());
                    if (refinedTerm.Length > 1)
                    {
                        retList.Add(refinedTerm);
                    }
                }

                Dictionary<String, int> most = retList.GroupBy(o => o).OrderByDescending(grp => grp.Count())
                    .Select(grp => grp).Take(NumOfTermsToTake)
                    .ToDictionary(r => r.Key, r => r.Count() * 1000);

                return most;
            }
            else
            {
                return null;
            }
        }
        
        public static DateTime GetLastestDateAndTime()
        {
            DateTime retval = new DateTime();

            try
            {
				DataTable dt = new DataTable();

				NpgsqlConnection conn = DBConnect.OpenConnection();
				NpgsqlCommand cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select max(createtime) from tweepy;";
				NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
				nda.Fill(dt);
                
                retval = DateTime.Parse(dt.Rows[0][0].ToString(), Tweet.culture, System.Globalization.DateTimeStyles.AssumeLocal);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBConnect.CloseConnection();
            }

            return retval;
        }
    }

}
