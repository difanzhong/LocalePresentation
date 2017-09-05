using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataCollection;
using DataPresentation.Models;
using Newtonsoft.Json.Linq;

namespace DataPresentation.Controllers
{
    public class PredictionController : Controller
    {
        // GET
        public ActionResult Index()
        {
            //get latest available datetime
            PredictionViewModel vm = new PredictionViewModel();

            DateTime latestDateTime = Tweet.GetLastestDateAndTime();

            vm.selectedDate = latestDateTime.Date;
            vm.selectedTime = latestDateTime.Hour;

            return View(vm);
        }

        public ActionResult Quest(PredictionViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Dictionary<int, double> rankDict = Tweet.GetMostSimilarResults(pvm.keywords, pvm.selectedTime,
                        pvm.selectedDate.ToShortDateString());
                    int[] suburbIdList = rankDict.Select(o => o.Key).ToArray();
                    string outlines = Suburb.getMultipleSuburbOutlines(suburbIdList);

                    if (!string.IsNullOrEmpty(outlines))
                    {
                        var data = JObject.Parse(outlines);

                        foreach (var d in data["feature"])
                        {
                            RankSuburbs rs = new RankSuburbs();
                            
                            rs.suburbName = d["f3"].ToString();
                            
                            string pattern = "/[0-9]+/";
                            Regex re = new Regex(pattern);
                            string replacement = "";
                            rs.stateNameAbbr = re.Replace(d["f2"].ToString(), replacement);

                            rs.score = Math.Round(Convert.ToDouble(d["f5"]), 3);
                            
                            pvm.rankSuburbList.Add(rs);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
        }
    }
}