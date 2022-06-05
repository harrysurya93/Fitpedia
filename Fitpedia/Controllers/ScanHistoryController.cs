using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class ScanHistoryController : Controller
    {
        // GET: ScanHistory
        public ActionResult Read(Fitpedia.Models.Structure.Message oMessage)
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;
                ViewBag.ScanHistory = Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.activity WHERE username = '{0}'", oLoginInformation.username));
                return View(oMessage);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }
    }
}