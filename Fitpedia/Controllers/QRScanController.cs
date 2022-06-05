using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class QRScanController : Controller
    {
        // GET: QRScan
        public ActionResult Create(Models.Structure.Message oMessage)
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;

                return View(oMessage);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }

        public ActionResult Scan(string deviceid)
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;

                if (Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.devices WHERE deviceid = '{0}' ", deviceid)).Count > 0)
                {
                    List<dynamic> Validation = Models.Connection.ExecuteSelectQuery("SELECT * FROM fitpedia.activity where username = '" + oLoginInformation.username + "' AND (YEAR(datetime) = " + DateTime.Now.Year + " AND MONTH(datetime) = " + DateTime.Now.Month + " AND DAY(datetime) = " + DateTime.Now.Day + ") " );
                    if(Validation.Count == 0)
                    {
                        List<dynamic> oScanId = Models.Connection.ExecuteSelectQuery("SELECT IFNULL(MAX(scanid),0)+1 AS scanid FROM fitpedia.scan_session");
                        Models.Connection.ExecuteEditQuery(String.Format("INSERT INTO fitpedia.scan_session (scanid, username, createdate, status, deviceid) VALUES ({0},'{1}', NOW(), FALSE, '{2}')", oScanId[0].scanid, oLoginInformation.username, deviceid));
                        ViewBag.deviceid = deviceid;
                        ViewBag.scanid = oScanId[0].scanid;
                        return View();

                    }
                    else
                    {
                        return RedirectToAction("Create", "QRScan", new Models.Structure.Message { MessageStatus = false, MessageText = "You already scan your body today, please delete your body scan for today to continue." });
                    }
                }
                else
                {
                    return RedirectToAction("Create", "QRScan", new Models.Structure.Message { MessageStatus = false, MessageText = "QR Code invalid." });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }
    }
}