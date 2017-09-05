using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace DataPresentation.Models
{
    public class PredictionViewModel : WordCloudViewModel
    {
        [Required]
        public string keywords { get; set; }
        
        public Dictionary<string, double> topResultsWithScore { get; set; }
        public string suburbOutlines { get; set; }
        public List<RankSuburbs> rankSuburbList { get; set; } 
    }

    public class RankSuburbs
    {
        public string suburbName { get; set; }
        public string stateNameAbbr { get; set; }
        public double score { get; set; }
    }
}