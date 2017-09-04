using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using DataCollection;
using System.Data;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using DataPresentation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataPresentation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            WordCloudViewModel vm = new WordCloudViewModel();

            DateTime latestDateTime = Tweet.GetLastestDateAndTime();

            vm.selectedDate = latestDateTime.Date;
            vm.selectedTime = latestDateTime.Hour;

            return View(vm);
        }

        [HttpPost]
        public ActionResult Quest(WordCloudViewModel wcvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    double longitude = Double.Parse(wcvm.longitude);
                    double latitude = Double.Parse(wcvm.latitude);

                    String jsonOutline = Suburb.getSuburbOutline(longitude, latitude);

                    if (!String.IsNullOrEmpty(jsonOutline))
                    {
                        var jsonObj = JObject.Parse(jsonOutline);
                        
                        if (jsonObj["features"] != null && jsonObj["features"].HasValues)
                        {
                            String suburbId = (string) jsonObj["features"][0]["properties"]["f1"];
                        
                            Dictionary<String, int> termsWithCount = Tweet.GetMostCommonWords(int.Parse(suburbId),
                                wcvm.selectedDate.ToShortDateString(), wcvm.selectedTime, 1500);
                            
                            if (termsWithCount != null && termsWithCount.Count > 0)
                            {
                                List<Dictionary<String, object>> wordsList = new List<Dictionary<string, object>>();

                                foreach (KeyValuePair<String, int> item in termsWithCount)
                                {
                                    Dictionary<String, object> tempDict = new Dictionary<string, object>();
                                    tempDict.Add("text", item.Key);
                                    tempDict.Add("size", item.Value);
                                    wordsList.Add(tempDict);
                                }

                                JavaScriptSerializer jss = new JavaScriptSerializer();
                                String wordslist = jss.Serialize(wordsList);

                                String output = "[" + jsonOutline + "," + wordslist + "]";

                                return Json(output);
                            }
                        }
                        
                        return Json("[" + jsonOutline + "]");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occur Post Quest");
                }
            }
            return null;
        }
    }
}
