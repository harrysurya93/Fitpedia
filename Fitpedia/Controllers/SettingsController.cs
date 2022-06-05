using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Read(Models.Structure.Message oMessage)
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

        public ActionResult ChangePassword(string OldPassword, string NewPassword)
        {
            try
            {
                if (!Models.Function.Authentication(this).MessageStatus) { return RedirectToAction("Index", "Main", Models.Function.Authentication(this)); }
                Models.Structure.LoginInformation oLoginInformation = ViewBag.LoginInformation;

                Models.Structure.Message oMessage = new Models.Structure.Message();
                if (Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.users WHERE username = '{0}' AND password = '{1}'", oLoginInformation.username, OldPassword)).Count > 0)
                {
                    Models.Connection.ExecuteEditQuery(String.Format("UPDATE fitpedia.users SET password = '{0}' WHERE username = '{1}'", NewPassword, oLoginInformation.username));
                    oMessage.MessageStatus = true;
                    oMessage.MessageText = "Password changed successfully, sign in to continue";
                    return RedirectToAction("Index", "Login", oMessage);
                }
                else
                {
                    oMessage.MessageStatus = false;
                    oMessage.MessageText = "Wrong old password.";
                    return RedirectToAction("Read", "Settings", oMessage);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Main", new Models.Structure.Message { MessageStatus = false, MessageText = ex.Message });
            }
        }
    }
}