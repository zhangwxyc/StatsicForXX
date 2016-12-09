using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSWeb.Controllers
{
    public class UserController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();

        public ActionResult Index()
        {
            ViewData["gInfo"] = DBContext.GroupInfo.ToList();
            return View();
        }

        [ActionName("s")]
        //[HttpGet]
        public JsonResult GetUserByGroup(string groupName)
        {
            var uInfos = DBContext.UserInfo.Where(x => x.GroupName == groupName);
            uInfos.ToList().ForEach(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.InTime))
                    {
                        x.InTime = "--";
                    }
                });
            return Json(uInfos, JsonRequestBehavior.AllowGet);
        }
        [ActionName("u")]
        public JsonResult GetUserById(long uid)
        {
            var uInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id == uid);
            return Json(uInfo, JsonRequestBehavior.AllowGet);
        }

        [ActionName("save")]
        public JsonResult OpUser(UserInfo userInfo)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (userInfo==null||userInfo.Id==0)
            {
                rInfo.Message = "参数不正确";
                return Json(rInfo);
            }
            try
            {
                var uInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id == userInfo.Id);
                if (uInfo == null)
                {
                    DBContext.UserInfo.Add(userInfo);
                }
                else
                {
                    uInfo.Name = userInfo.Name;
                    uInfo.InTime = userInfo.InTime;
                    uInfo.GroupName = userInfo.GroupName;
                    uInfo.IsDel = 0;
                }

                DBContext.SaveChanges();
                rInfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
            }

            return Json(rInfo);
        }
    }
}
