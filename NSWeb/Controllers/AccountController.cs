using NSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NSWeb.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public ActionResult Login(string Password, string returnUrl)
        {
            returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/home/index" : returnUrl;
            if (ModelState.IsValid && Password.ToLowerInvariant() == System.Configuration.ConfigurationManager.AppSettings["AccessKey"].ToLowerInvariant())
            {
                FormsAuthentication.SetAuthCookie("qiqi", false);
                return Redirect(returnUrl);
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ViewBag.Error = "口令不正确";
            ViewBag.ReturnUrl= returnUrl;
            return View();
        }
    }
}
