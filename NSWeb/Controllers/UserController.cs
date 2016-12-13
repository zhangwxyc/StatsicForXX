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
            var infos = DBContext.GroupInfo.Where(x => x.IsDel != 1).ToList();
            infos.ForEach(x => x.ParentName = GetGoodName(x.Name));
            ViewData["gInfo"] = infos;
            return View();
        }

        private string GetGoodName(string name)
        {
            string firstL = StatsisLib.PinYinHelper.GetChineseSpell(name);
            return string.Format("{0} {1}", firstL.Substring(0, 1), name);
        }

        [ActionName("s")]
        //[HttpGet]
        public JsonResult GetUserByGroup(string groupName)
        {
            var uInfos = DBContext.UserInfo.Where(x => x.IsDel != 1 && x.GroupName == groupName);
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
        [ActionName("del")]
        public JsonResult DelUserById(long uid)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            try
            {
                var uInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id == uid);
                if (uInfo != null)
                {
                    uInfo.IsDel = 1;
                    DBContext.SaveChanges();
                    rInfo.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
            }

            return Json(rInfo);
        }
        [ActionName("save")]
        public JsonResult OpUser(UserInfo userInfo)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (userInfo == null || userInfo.Id == 0)
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
