using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Create(Models.Structure.Message oMessage)
        {
            return View(oMessage);
        }

        public ActionResult Verify(string Username, string Password, string Telephone, string Sex, string FullName, int Height)
        {
            Models.Structure.Message oMessage = new Models.Structure.Message();
            if (Models.Connection.ExecuteSelectQuery(String.Format("SELECT * FROM fitpedia.users WHERE username = '{0}'", Username)).Count > 0)
            {
                oMessage.MessageStatus = false;
                oMessage.MessageText = "User with this username already exists";
                return RedirectToAction("Create", "Register", oMessage);
            }
            else
            {
                Models.Connection.ExecuteEditQuery(String.Format("INSERT INTO fitpedia.users (username,password,telephone,sex,name, createdate, height) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', NOW(),{6})", Username, Password, Telephone, Sex, FullName, Height));
                oMessage.MessageStatus = true;
                oMessage.MessageText = "Success registering new account, please sign in.";
                return RedirectToAction("Index", "Login", oMessage);
            }
        }
    }
}