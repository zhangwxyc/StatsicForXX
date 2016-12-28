using DataService;
//using StatsisLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NSWeb.Controllers
{
    public class UserController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();

        [Authorize]
        public ActionResult Index()
        {
            var infos = DBContext.GroupInfo.Where(x => x.IsDel != 1).OrderBy(x => x.OrderIndex).ToList();
            infos.ForEach(x => x.ParentName = GetGoodName(x.Name));
            ViewData["gInfo"] = infos;
            return View();
        }
        [Authorize]
        public ActionResult Check()
        {
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
            var uInfos = DBContext.UserInfo.Where(x => x.IsDel != 1 && x.GroupName == groupName).OrderBy(x => x.OrderIndex);
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
                int maxIndex = 1;
                if (DBContext.UserInfo.Count(x => x.GroupName == userInfo.GroupName) != 0)
                {
                    maxIndex = DBContext.UserInfo.Where(x => x.GroupName == userInfo.GroupName).Max(x => x.OrderIndex).Value + 1;
                }

                var uInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id == userInfo.Id);
                if (uInfo == null)
                {
                    userInfo.OrderIndex = maxIndex;
                    DBContext.UserInfo.Add(userInfo);
                }
                else
                {
                    uInfo.Name = userInfo.Name;
                    uInfo.InTime = userInfo.InTime;
                    uInfo.IsShield = userInfo.IsShield;

                    if (uInfo.GroupName != userInfo.GroupName)
                    {
                        uInfo.OrderIndex = maxIndex;
                        uInfo.GroupName = userInfo.GroupName;
                    }
                    uInfo.Remark = userInfo.Remark ?? "";
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

        [ActionName("op")]
        public JsonResult OpGroup(string name, int op)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (string.IsNullOrWhiteSpace(name))
            {
                rInfo.Message = "参数不正确";
                return Json(rInfo);
            }

            try
            {

                var currentInfo = DBContext.GroupInfo.FirstOrDefault(x => x.Name == name);
                if (op == 1)
                {
                    if (currentInfo == null)
                    {
                        currentInfo = new GroupInfo() { Name = name, IsDel = 0 };
                        DBContext.GroupInfo.Add(currentInfo);
                        DBContext.SaveChanges();
                        rInfo.IsSuccess = true;
                    }
                    else if (currentInfo.IsDel == 1)
                    {
                        currentInfo.IsDel = 0;
                        DBContext.SaveChanges();
                        rInfo.IsSuccess = true;
                    }
                    else
                    {
                        rInfo.Message = string.Format("{0} 已存在", name);
                    }
                }
                else
                {
                    if (currentInfo != null)
                    {
                        currentInfo.IsDel = 1;
                        DBContext.SaveChanges();
                        rInfo.IsSuccess = true;
                    }
                    else
                    {
                        rInfo.Message = string.Format("没有 {0}", name);
                    }
                }
            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
            }

            return Json(rInfo);
        }

        [ActionName("upload")]
        public JsonResult UpLoad()
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            try
            {
                var files = Request.Files;
                if (files == null && files.Count != 1)
                {
                    rInfo.Message = "文件为空或者个数不正确";
                }

                HttpPostedFileBase fileItem = Request.Files[0] as HttpPostedFileBase;

                if (fileItem != null && fileItem.ContentLength > 0)
                {
                    StringBuilder str = new StringBuilder();

                    string saveName = string.Format("{0}_{1}", DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace("/", "_").Replace(" ", "_"), fileItem.FileName);


                    var dt = StatsisLib.NPOIHelper.ImportExceltoDt(fileItem.InputStream);
                    var urInfos = StatsisLib.Common.DTToList<StatsisLib.Models.URInfo>(dt);

                    var gInfos = urInfos.Where(x => !x.组别.Contains(x.姓名)).GroupBy(x => x.组别).ToDictionary(x => x.Key, x => x.ToList());
                    int groupIndex = 1;
                    foreach (var item in gInfos)
                    {
                        var gInfo = DBContext.GroupInfo.FirstOrDefault(x => x.Name == item.Key);
                        if (gInfo == null)
                        {
                            str.AppendFormat("缺少组：{0}", item.Key);
                            gInfo = new GroupInfo()
                            {
                                Name = item.Key,
                                IsDel = 0,
                                OrderIndex = groupIndex
                            };
                        }
                        else
                        {
                            gInfo.OrderIndex = groupIndex;
                            gInfo.IsDel = 0;
                        }

                        #region u
                        int index = 1;
                        foreach (var uItem in item.Value)
                        {
                            var s = DBContext.UserInfo.FirstOrDefault(x => x.Id.ToString() == uItem.工号);
                            if (s == null)
                            {
                                s = new UserInfo()
                                {
                                    Id = int.Parse(uItem.工号),
                                    GroupName = uItem.组别,
                                    Name = uItem.姓名,
                                    OrderIndex = index,
                                    Remark = uItem.特殊情况,
                                    IsDel = 0
                                };
                                DBContext.UserInfo.Add(s);
                                str.AppendFormat("缺少：{0}", uItem.工号);
                            }
                            else if (s.GroupName != uItem.组别)
                            {
                                s.IsDel = 0;

                                s.GroupName = uItem.组别;
                                str.AppendFormat("对不上：{0},{1}=>{2}", uItem.工号, s.GroupName, uItem.组别);
                            }

                            s.Remark += uItem.特殊情况;
                            s.OrderIndex = index;

                            index++;
                        }
                        groupIndex++;

                        #endregion
                    }



                    foreach (var dItem in DBContext.UserInfo.ToList())
                    {
                        var s = urInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                        if (s == null && (dItem.IsDel??0) == 0)
                        {
                            dItem.IsDel = 1;
                            str.AppendFormat("多余：{0}", dItem.Id, dItem.GroupName);
                        }
                    }

                    DBContext.SaveChanges();
                    rInfo.Message = str.ToString();
                    if (string.IsNullOrWhiteSpace(rInfo.Message))
                    {
                        rInfo.Message = "没有更改";
                    }
                    rInfo.IsSuccess = true;
                }


            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
                Json(rInfo);
            }
            return Json(rInfo);
        }

        [ActionName("changePostion")]
        public JsonResult ChangePostion(int id, int op)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            //if (string.IsNullOrWhiteSpace(name))
            //{
            //    rInfo.Message = "参数不正确";
            //    return Json(rInfo);
            //}

            try
            {
                var currentInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id == id);
                if (currentInfo == null)
                {
                    rInfo.Message = "参数不正确";
                    return Json(rInfo);
                }
                var nextList = DBContext.UserInfo.Where(x => x.GroupName == currentInfo.GroupName
                    && (op == 1 ? (x.OrderIndex.Value > currentInfo.OrderIndex.Value) : (x.OrderIndex.Value < currentInfo.OrderIndex.Value)));
                if (nextList.Count() == 0)
                {
                    rInfo.Message = "无需移动";
                    return Json(rInfo);
                }
                var nextInfo = new UserInfo();
                if (op == 1)
                {
                    nextInfo = nextList.OrderBy(x => x.OrderIndex).FirstOrDefault();
                }
                else
                {
                    nextInfo = nextList.OrderByDescending(x => x.OrderIndex).FirstOrDefault();
                }

                int? tmpIndex = currentInfo.OrderIndex;
                currentInfo.OrderIndex = nextInfo.OrderIndex;
                nextInfo.OrderIndex = tmpIndex;

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
