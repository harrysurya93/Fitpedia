using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class AjaxController : Controller
    {
        // GET: Ajax
        public JsonResult DeleteActivity(int id)
        {
            Models.Structure.Message oMessage = new Models.Structure.Message();
            try
            {
                Models.Connection.ExecuteEditQuery(String.Format("DELETE FROM fitpedia.activity WHERE activityid = {0}", id));
                oMessage.MessageStatus = true;
                oMessage.MessageText = "Success deleting record";
                return Json(oMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oMessage.MessageStatus = false;
                oMessage.MessageText = ex.Message;
                return Json(oMessage, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult InsertActivity(string username, int scanid, string deviceid)
        {
            Models.Structure.Message oMessage = new Models.Structure.Message();
            try
            {
                dynamic Scan_Session = Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.scan_session WHERE username = '{0}' AND scanid = {1} AND deviceid = '{2}'", username, scanid, deviceid));
                if(Scan_Session.Count > 0)
                {
                    //Ambil data IoT dalam 3 detik terakhir
                    List<double> zWeight = new List<double>();
                    string query = String.Format("SELECT value FROM fitpedia.iot_weight WHERE deviceid='{0}' AND createdate BETWEEN '{1}' AND '{2}' ORDER BY createdate ASC", deviceid, Scan_Session[0].createdate.AddSeconds(-2).ToString("yyyy-MM-dd HH:mm:ss"), Scan_Session[0].createdate.ToString("yyyy-MM-dd HH:mm:ss"));
                    foreach (dynamic Weight in Models.Connection.ExecuteSelectQuery(query))
                    {
                        zWeight.Add(Weight.value);
                    }

                    
                    double[] Outliers = Models.Function.RemoveOutliers(zWeight).ToArray();
                    //Gunakan EMA untuk smooth data noise
                    double[] EMA = Models.Function.EMA(Outliers, Outliers.Length);
                    //Gunakan Average untuk menentukan nilai akhir
                    double Average = Queryable.Average(EMA.AsQueryable());

                    Models.Connection.ExecuteEditQuery(String.Format("INSERT INTO fitpedia.activity (username, weight, datetime, deviceid) VALUES('{0}', {1} , '{2}' , '{3}') ", username, Math.Round(Average / 1000, 1), Scan_Session[0].createdate.ToString("yyyy-MM-dd HH:mm:ss"), deviceid));
                    Models.Connection.ExecuteEditQuery(String.Format("UPDATE fitpedia.scan_session SET status= 'TRUE' WHERE scanid = {0} ", scanid));

                    oMessage.MessageStatus = true;
                    oMessage.MessageText = "Success scan weight information.";
                    return Json(oMessage, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Session not found");
                }
            }
            catch (Exception ex)
            {
                oMessage.MessageStatus = false;
                oMessage.MessageText = ex.Message;
                return Json(oMessage, JsonRequestBehavior.AllowGet);
            }
        }

    }
}