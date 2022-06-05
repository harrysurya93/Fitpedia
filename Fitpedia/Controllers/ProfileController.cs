using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Read()
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;
                List<dynamic> Output = Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.users WHERE username = '{0}'", oLoginInformation.username));
                return View(Output);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }
    }
}