using Fitpedia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fitpedia.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(Structure.Message oMessage)
        {
            Session.RemoveAll();
            if(oMessage.MessageText != null)
            {
                return View(oMessage);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Verify(string Username, string Password)
        {
            Structure.Message oMessage = Function.Authorization(Username, Password);
            if (oMessage.MessageStatus)
            {                
                return RedirectToAction("Index","Main");
            }
            else
            {
                return RedirectToAction("Index","Login", oMessage);
            }
        }
    }
}