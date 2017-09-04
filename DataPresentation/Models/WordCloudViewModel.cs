using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataPresentation.Models
{
    public class WordCloudViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy", ApplyFormatInEditMode = true)]
        public DateTime selectedDate { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        public int selectedTime { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        public String latitude { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        public String longitude { get; set; }
        
        public Dictionary<String, int> topWordsWithFrequency { get; set; }
        public Dictionary<int, String> TimeSelections
        {
            get
            {
                return this.GetTimeSelections();
            }
            set { }
        }

        private Dictionary<int, String> GetTimeSelections()
        {
            Dictionary<int, String> retVal = new Dictionary<int, String>();
            for (int i = 0; i < 24; i++)
            {
                retVal.Add(i, i.ToString("00")+":00");
            }
            return retVal;
        }
    }
}
