using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DataCollection.Stem;

namespace DataCollection
{
    class Calculation
    {
        private EnglishStemmer _Stemmer { get; set; }
        private List<Dictionary<string,object>> _docList { get; set; }
        private Dictionary<string, double> _idfDict { get; set; }
        private List<String> _termsList { get; set; }
        private Dictionary<int, List<double>> _tfIdfTable { get; set; }

        
        /*
            It used to transform query string into words array 
            and model them into document
            it returns a list of double as the tfidf weighted value
        */
        public List<double> generateQueryDoc(string query)
        {
            List<string> queryWords = query.Split(' ').Select(o => this._Stemmer.Stem(o.Trim())).ToList();
            List<double> tfidfList = new List<double>();

            if (this._termsList != null && this._termsList.Count > 0)
            {
                foreach (string t in this._termsList)
                {
                    int occurance = this.findOccurance(queryWords.ToArray(), t);
                    double tf = Convert.ToDouble(occurance);
                    double idf = 0;

                    if (this._idfDict != null && this._idfDict.Count > 0)
                    {
                        idf = this._idfDict[t];
                    }
                    
                    tfidfList.Add(tf * idf);
                }
            }

            return tfidfList;
        }

        /*
            It takes queries and all suburbs with terms in an hour
            and it returns 5 most similar documents, which are suburbs,
            after a series of calculation,
        */
        public Dictionary<int, double> top5RankedCosSimilarity(List<double> queryDoc, Dictionary<string, ArrayList> termsAndSuburbs)
        {
            Dictionary<int, double> cosSimRank = new Dictionary<int, double>();
            this.arrangeDocumentCollection(termsAndSuburbs);
            this.createTfIdfTable();
            Dictionary<int, List<double>> tfIdfTable = this._tfIdfTable;
            
            foreach (KeyValuePair<int, List<double>> item in tfIdfTable)
            {
                double cosSimilarity = this.cosineSimilarity(queryDoc, item.Value);
                cosSimRank.Add(item.Key, cosSimilarity);
            }

            return cosSimRank.OrderByDescending(o=>o.Value).Take(5).ToDictionary(o=>o.Key, o=>o.Value);
        }

        private double cosineSimilarity(List<double> query, List<double> doc)
        {
            double upper = 0;
            double lower = 0;

            double lowerLeft = 0;
            double lowerRight = 0;

            for (int i = 0; i < doc.Count; i++)
            {
                upper += query[i] * doc[i];
                lowerLeft += Math.Pow(query[i], 2);
                lowerRight += Math.Pow(doc[i], 2);
            }

            double cosineSimilarity = 0;
            lower = lowerLeft * lowerRight;

            if (lower != 0)
            {
                cosineSimilarity = upper / Math.Sqrt(lower);
            }
            return cosineSimilarity;
        }
        
        private void arrangeDocumentCollection(Dictionary<string, ArrayList> termsAndSuburbs)
        {
            if (termsAndSuburbs != null && termsAndSuburbs.Count > 0)
            {
                if (termsAndSuburbs["document"] != null && termsAndSuburbs["document"].Count > 0)
                {
                    // re-arranged structure, split and stemmed the words into list 
                    // and put them with document in docList
                    foreach (Dictionary<string, object> document in termsAndSuburbs["document"])
                    {
                        string words = document["words"].ToString();
                        words = words.Replace("'", "").Replace("#", "");
                        string[] wordsArr = words.Split(',').Where(o=>_Stemmer.Stem(o.Trim()).Length > 1).Select(o => this._Stemmer.Stem(o.Trim())).ToArray();
                        document["words"] = wordsArr;
                        
                        this._docList.Add(document);
                    }
                }

                if (termsAndSuburbs["words"] != null && termsAndSuburbs.Count > 0)
                {
                    Dictionary<string, int> distinctWords =
                        this.refineAndGroupTerms(((string[])termsAndSuburbs["words"].ToArray()).ToList(), 1500);
                    foreach (string term in distinctWords.Keys)
                    {
                        this._termsList.Add(term);
                    }
                }
            }
        }

        private void createTfIdfTable()
        {
            Dictionary<int, List<double>> docTfIdf = new Dictionary<int, List<double>>();
            
            try
            {
                List<object> tfIdfMap = new List<object>();
                Dictionary<string, double> idfTable = new Dictionary<string, double>();

                if (this._termsList != null && this._termsList.Count > 0)
                {
                    foreach (string term in this._termsList)
                    {
                        Dictionary<int, double> termTfIdf = new Dictionary<int, double>();
                        int numOfDocContains = 0;

                        foreach (Dictionary<string, object> doc in this._docList)
                        {
                            string[] textArr = (string[]) doc["words"];
                            int occurance = findOccurance(textArr, term);

                            if (occurance > 0)
                            {
                                numOfDocContains += 1;
                            }

                            // occurance is actually the term frequency
                            int termFrequency = occurance;

                            int docId = Convert.ToInt32(doc["suburbId"]);

                            // assigning term frequency to its document (suburb)
                            termTfIdf.Add(docId, Convert.ToDouble(termFrequency));
                        }

                        int totalNumOfDocs = this._docList.Count;

                        // calculating idf
                        double div = Convert.ToDouble(totalNumOfDocs) / Convert.ToDouble(numOfDocContains);
                        double idf = Math.Log(div, 2);

                        idfTable.Add(term, idf);

                        foreach (KeyValuePair<int, double> item in termTfIdf)
                        {
                            termTfIdf[item.Key] = item.Value * idf;
                        }

                        tfIdfMap.Add(termTfIdf);
                    }

                    this._idfDict = idfTable;
                }

                

                if (this._docList != null && this._docList.Count > 0)
                {
                    foreach (Dictionary<string, object> doc in this._docList)
                    {
                        List<double> tfIdfForDoc = new List<double>();

                        int docId = Convert.ToInt32(doc["suburbId"]);

                        foreach (Dictionary<int, double> term in tfIdfMap)
                        {
                            tfIdfForDoc.Add(term[docId]);
                        }

                        docTfIdf[docId] = tfIdfForDoc;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            
            this._tfIdfTable = docTfIdf;
        }

        private int findOccurance(string[] text, string term)
        {
            int counter = 0;

            foreach (string t in text)
            {
                if (t == term)
                {
                    counter+=1;
                }
            }

            return counter;
        }

        private Dictionary<string, int> refineAndGroupTerms(List<string> terms, int num)
        {
            List<String> retList = new List<string>();
            if (terms != null && terms.Length > 0)
            {
                foreach (string t in terms)
                {
                    String refinedTerm = new EnglishStemmer().Stem(t.Trim());
                    if (refinedTerm.Length > 1)
                    {
                        retList.Add(refinedTerm);
                    }
                }

                if (num > 0)
                {
                    Dictionary<String, int> most = retList.GroupBy(o => o).OrderByDescending(grp => grp.Count())
                        .Select(grp => grp).Take(num)
                        .ToDictionary(r => r.Key, r => r.Count() * 1000);

                    return most;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
    }
}