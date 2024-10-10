using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HRManager.Controllers
{
    public class SendMailerController : Controller
    {
        // GET: SendMailer
        public ActionResult Index()
        {
            return View();
        }
    }
}