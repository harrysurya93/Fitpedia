using Fitpedia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        public ActionResult Read()
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;
                List<dynamic> SourceData = Fitpedia.Models.Connection.ExecuteSelectQuery("SELECT datetime, ROUND(weight / POWER(height / 100, 2), 3) AS BMI FROM fitpedia.activity INNER JOIN fitpedia.users ON fitpedia.users.username = fitpedia.activity.username WHERE fitpedia.activity.username = '" + oLoginInformation.username + "'");
                if(SourceData.Count > 0)
                {

                    List<Models.Structure.BMI_Graphic> oInput = new List<Structure.BMI_Graphic>();
                    foreach (dynamic value in SourceData)
                    {
                        oInput.Add(new Structure.BMI_Graphic { datetime = value.datetime.ToString("dd-MM-yyyy"), BMI = float.Parse(value.BMI.ToString()) });
                    }
                    ViewBag.RawData = oInput;
                    ViewBag.LatestDate = Fitpedia.Models.Connection.ExecuteSelectQuery("SELECT MAX(datetime) AS value FROM fitpedia.activity INNER JOIN fitpedia.users ON fitpedia.users.username = fitpedia.activity.username WHERE fitpedia.activity.username = '" + oLoginInformation.username + "'");
                    return View(Models.Function.MachineLearning.TimeSeriesForecast(oLoginInformation.username));
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }
    }
}