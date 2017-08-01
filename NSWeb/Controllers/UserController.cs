﻿using DataService;
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
                    uInfo.IsTrimFromGroup = userInfo.IsTrimFromGroup;

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

                    var ds = StatsisLib.NPOIHelper.ImportExceltoDs(fileItem.InputStream);

                    List<StatsisLib.Models.URInfo> sheildInfos = new List<StatsisLib.Models.URInfo>();
                    if (ds.Tables.Count > 1)
                    {
                        sheildInfos = StatsisLib.Common.DTToList<StatsisLib.Models.URInfo>(ds.Tables[1]);
                    }


                    var dt = ds.Tables[0];
                    var urInfos = StatsisLib.Common.DTToList<StatsisLib.Models.URInfo>(dt);

                    if (fileItem.FileName.Contains("time"))
                    {
                        //更新时间
                        str.Append("更新用户时间处理<br/>");
                        foreach (var item in urInfos)
                        {
                            var uuInfo = DBContext.UserInfo.FirstOrDefault(x => x.Id.ToString() == item.工号);
                            if (uuInfo != null)
                            {
                                if (!string.IsNullOrWhiteSpace(item.入组时间))
                                {
                                    uuInfo.InTime = item.入组时间;
                                }

                                if (!string.IsNullOrWhiteSpace(item.特殊情况))
                                {
                                    uuInfo.Remark = item.特殊情况;
                                }

                                str.AppendFormat("更新用户：{0}<br/>", item.姓名);
                            }
                        }
                    }
                    else
                    {


                        ///去掉组长本人
                        var gInfos = urInfos.Where(x => !x.组别.Contains(x.姓名)).GroupBy(x => x.组别).ToDictionary(x => x.Key, x => x.ToList());
                        int groupIndex = 1;
                        foreach (var item in gInfos)
                        {
                            var gInfo = DBContext.GroupInfo.FirstOrDefault(x => x.Name == item.Key);
                            if (gInfo == null)
                            {

                                str.AppendFormat("缺少组：{0}<br/>", item.Key);
                                gInfo = new GroupInfo()
                                {
                                    Name = item.Key,
                                    IsDel = 0,
                                    OrderIndex = groupIndex,
                                    IsLeaf = 1
                                };
                                DBContext.GroupInfo.Add(gInfo);
                            }
                            else
                            {
                                gInfo.OrderIndex = groupIndex;
                                gInfo.IsDel = 0;

                            }
                            groupIndex++;

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
                                    str.AppendFormat("缺少：{0}<br/>", uItem.工号);
                                }
                                else if (s.GroupName != uItem.组别)
                                {
                                    str.AppendFormat("对不上：{0},{1}=>{2}<br/>", uItem.工号, s.GroupName, uItem.组别);
                                    s.GroupName = uItem.组别;
                                }
                                if (s.IsShield == 1)
                                {
                                    str.AppendFormat("恢复：{0},{1}<br/>", uItem.组别, uItem.工号);
                                }
                                s.IsDel = 0;
                                s.IsShield = 0;
                                s.IsTrimFromGroup = 0;
                                s.Remark = uItem.特殊情况;
                                s.OrderIndex = index;
                                //s.InTime = uItem.入组时间;
                                index++;
                            }

                            #endregion

                        }



                        foreach (var dItem in DBContext.UserInfo.Where(x => x.IsDel == 0).ToList())
                        {
                            var s = urInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                            if (s == null && (dItem.IsShield ?? 0) == 0)
                            {
                                var sheildItem = sheildInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                                if (sheildItem != null)
                                {
                                    dItem.IsShield = 1;
                                    dItem.Remark = sheildItem.特殊情况;
                                    str.AppendFormat("不考核：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                                }
                                else
                                {
                                    dItem.IsDel = 1;
                                    dItem.IsShield = 1;
                                    str.AppendFormat("多余（将删除）：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                                }
                            }
                            else if (s == null && (dItem.IsShield ?? 0) == 1)
                            {
                                var sheildItem = sheildInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                                if (sheildItem == null)
                                {
                                    dItem.IsDel = 1;
                                    dItem.IsShield = 1;
                                    str.AppendFormat("清理（将删除）：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                                }
                            }
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

        [ActionName("upload2")]
        public JsonResult UpLoad2()
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

                    var ds = StatsisLib.NPOIHelper.ImportExceltoDs(fileItem.InputStream);

                    var dt = ds.Tables[0];
                    var urInfos = StatsisLib.Common.DTToList<StatsisLib.Models.URInfo>(dt);

                    List<StatsisLib.Models.URInfo> sheildInfos = new List<StatsisLib.Models.URInfo>();
                    if (ds.Tables.Count > 1)
                    {
                        sheildInfos = StatsisLib.Common.DTToList<StatsisLib.Models.URInfo>(ds.Tables[1]);

                        foreach (var item in sheildInfos)
                        {
                            item.IsHiddenFromInGroup = true;
                        }
                        urInfos.AddRange(sheildInfos);
                    }

                    ///去掉组长本人
                    var gInfos = urInfos.Where(x => !x.组别.Contains(x.姓名)).GroupBy(x => x.组别).ToDictionary(x => x.Key, x => x.ToList());
                    int groupIndex = 1;
                    foreach (var item in gInfos)
                    {
                        var gInfo = DBContext.GroupInfo.FirstOrDefault(x => x.Name == item.Key);
                        if (gInfo == null)
                        {

                            str.AppendFormat("缺少组：{0}<br/>", item.Key);
                            gInfo = new GroupInfo()
                            {
                                Name = item.Key,
                                IsDel = 0,
                                OrderIndex = groupIndex,
                                IsLeaf = 1
                            };
                            DBContext.GroupInfo.Add(gInfo);
                        }
                        else
                        {
                            gInfo.OrderIndex = groupIndex;
                            gInfo.IsDel = 0;

                        }
                        groupIndex++;

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
                                    IsDel = 0,

                                };
                                DBContext.UserInfo.Add(s);
                                str.AppendFormat("缺少：{0}<br/>", uItem.工号);
                            }
                            else if (s.GroupName != uItem.组别)
                            {
                                str.AppendFormat("对不上：{0},{1}=>{2}<br/>", uItem.工号, s.GroupName, uItem.组别);
                                s.GroupName = uItem.组别;
                            }
                            if (s.IsShield == 1)
                            {
                                str.AppendFormat("恢复：{0},{1}<br/>", uItem.组别, uItem.工号);
                            }
                            s.IsDel = 0;
                            s.IsShield = 0;
                            s.IsTrimFromGroup = uItem.IsHiddenFromInGroup ? 1 : 0;
                            s.Remark = uItem.特殊情况;
                            s.OrderIndex = index;
                            if (string.IsNullOrWhiteSpace(s.InTime))
                            {
                                s.InTime = GetInGroupTime(uItem.入组时间);
                            }
                            //if (string.IsNullOrWhiteSpace(s.InTime))
                            //{
                            var tTime = GetInGroupTime(uItem.特殊情况);
                            if (!string.IsNullOrWhiteSpace(tTime))
                            {
                                s.InTime = tTime;
                            }
                            //}
                            index++;
                        }

                        #endregion

                    }




                    foreach (var dItem in DBContext.UserInfo.Where(x => x.IsDel == 0).ToList())
                    {
                        var s = urInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                        if (s == null && (dItem.IsShield ?? 0) == 0)
                        {
                            //var sheildItem = sheildInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                            //if (sheildItem != null)
                            //{
                            dItem.IsShield = 1;
                            // dItem.Remark = sheildItem.特殊情况;
                            str.AppendFormat("不考核：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                            //}
                            //else
                            //{
                            //    dItem.IsDel = 1;
                            //    dItem.IsShield = 1;
                            //    str.AppendFormat("多余（将删除）：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                            //}
                        }
                        else if (s == null && (dItem.IsShield ?? 0) == 1)
                        {
                            var sheildItem = sheildInfos.FirstOrDefault(x => x.工号 == dItem.Id.ToString());
                            if (sheildItem == null)
                            {
                                dItem.IsDel = 1;
                                dItem.IsShield = 1;
                                str.AppendFormat("清理（将删除）：{1}——{0}<br/>", dItem.Id, dItem.GroupName);
                            }
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

        private string GetInGroupTime(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
            {
                return "";
            }
            p = p.Replace("入组", "");
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(p, out dt))
            {
                return p;
            }
            return "";
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
