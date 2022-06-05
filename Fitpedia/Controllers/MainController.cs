using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Index(Models.Structure.Message oMessage)
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Login", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;

                return View(oMessage);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }

        public ActionResult BulkScan()
        {
            try
            {
                List<dynamic> Scan_Session = Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.scan_session WHERE status = 0 AND createdate >= '2021-09-24 12:05:58'"));
                foreach(dynamic value in Scan_Session)
                {
                    //Ambil data IoT dalam 3 detik terakhir
                    List<double> zWeight = new List<double>();
                    foreach (dynamic Weight in Models.Connection.ExecuteSelectQuery(String.Format("SELECT value FROM fitpedia.iot_weight WHERE deviceid='{0}' AND createdate BETWEEN '{1}' AND '{2}' ORDER BY createdate ASC", value.deviceid, value.createdate.ToString("yyyy-MM-dd HH:mm:ss"), value.createdate.AddSeconds(5).ToString("yyyy-MM-dd HH:mm:ss"))))
                    //foreach (dynamic Weight in Models.Connection.ExecuteSelectQuery(String.Format("SELECT value FROM fitpedia.iot_weight WHERE deviceid='{0}' AND createdate = '{1}' ORDER BY createdate ASC", value.deviceid, value.createdate.ToString("yyyy-MM-dd HH:mm:ss"))))
                    {
                        zWeight.Add(Weight.value);
                    }
                    if(zWeight.Count > 0)
                    {
                        //Cari dan buang Outliers
                        double[] Outliers = Models.Function.RemoveOutliers(zWeight).ToArray();
                        //Gunakan EMA untuk smooth data noise
                        double[] EMA = Models.Function.EMA(Outliers, Outliers.Length);
                        //Gunakan Average untuk menentukan nilai akhir
                        double Average = Queryable.Average(EMA.AsQueryable());

                        Models.Connection.ExecuteEditQuery(String.Format("INSERT INTO fitpedia.activity (username, weight, datetime, deviceid) VALUES('{0}', {1} , '{2}' , '{3}') ", value.username, Math.Round(Average / 1000, 1), value.createdate.ToString("yyyy-MM-dd HH:mm:ss"), value.deviceid));
                        Models.Connection.ExecuteEditQuery(String.Format("UPDATE fitpedia.scan_session SET status=TRUE WHERE scanid = {0} ", value.scanid));
                    }
                }
                return Content("Success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}