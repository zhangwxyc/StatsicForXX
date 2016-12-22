using DataService;
using NSWeb.Common;
using NSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSWeb.Controllers
{
    public class StatsicController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();

        public ActionResult Index()
        {
            var sInfos = DBContext.StatsicInfo.Where(x => x.IsDel != 1).ToList();
            var gInfos = DBContext.GroupInfo.Where(x => x.IsDel != 1).ToList();

            gInfos.ForEach(x => x.ParentName = GetGoodName(x.Name));
            var infos = sInfos.Select(x => new StatsicsViewModels()
                {
                    StatsicLineName = x.StatsicName,
                    Datas = GetDatas(x.StatsicName, gInfos)
                }).ToList();

            return View(infos);
        }

        private List<GroupItemModel> GetDatas(string name, List<GroupInfo> gInfos)
        {
            var selectedList = DBContext.StatsicRelation.Where(x => x.StatsicName == name).Select(x => x.GroupName).ToList();
            return GetDatas(selectedList, gInfos);
        }

        private List<GroupItemModel> GetDatas(List<string> selectedList, List<GroupInfo> gInfos)
        {
            List<GroupItemModel> infos = new List<GroupItemModel>();
            foreach (var item in gInfos)
            {
                infos.Add(new GroupItemModel()
                {
                    IsSelected = selectedList.Contains(item.Name) ? 1 : 0,
                    Info = item,
                    Tags = STag.GetTags(item.MapperName)
                });
            }
            return infos;
        }
        private string GetGoodName(string name)
        {
            string firstL = StatsisLib.PinYinHelper.GetChineseSpell(name);
            //return firstL;
            return string.Format("{0} {1}", firstL.Substring(0, 1), name);
        }
        //private Dictionary<GroupInfo, int> GetDatas(string name, List<GroupInfo> gInfos)
        //{
        //    var selectedList = DBContext.StatsicRelation.Where(x => x.StatsicName == name).Select(x => x.GroupName).ToList();
        //    return GetDatas(selectedList, gInfos);
        //}
        //private Dictionary<GroupInfo, int> GetDatas(List<string> list, List<DataService.GroupInfo> gInfos)
        //{
        //    Dictionary<GroupInfo, int> infos = new Dictionary<GroupInfo, int>();
        //    foreach (var item in gInfos)
        //    {
        //        infos.Add(item, list.Contains(item.Name) ? 1 : 0);
        //    }
        //    return infos;
        //}

        public static void GetData(List<GroupItemModel> models)
        {
            foreach (var tTag in NSWeb.Common.STag.TypeTags)
            {
                foreach (var dTag in NSWeb.Common.STag.DomainTags)
                {

                }
            }
        }

        [ActionName("op")]
        public JsonResult AddStatsicLine(string statsicName, int op)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (string.IsNullOrWhiteSpace(statsicName))
            {
                rInfo.Message = "参数不正确";
                return Json(rInfo);
            }

            try
            {

                var currentInfo = DBContext.StatsicInfo.FirstOrDefault(x => x.StatsicName == statsicName);
                if (op == 1)
                {
                    if (currentInfo == null)
                    {
                        currentInfo = new StatsicInfo() { StatsicName = statsicName, IsDel = 0 };
                        DBContext.StatsicInfo.Add(currentInfo);
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
                        rInfo.Message = string.Format("{0} 已存在", statsicName);
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
                        rInfo.Message = string.Format("没有 {0}", statsicName);
                    }
                }
            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
            }

            return Json(rInfo);
        }


        [ActionName("save")]
        public JsonResult SaveRelation(string statsicName, List<string> infos)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (string.IsNullOrWhiteSpace(statsicName) || infos == null)
            {
                rInfo.Message = "参数不正确";
                return Json(rInfo);
            }

            try
            {

                var currentInfos = DBContext.StatsicRelation.Where(x => x.StatsicName == statsicName);
                DBContext.StatsicRelation.RemoveRange(currentInfos);
                foreach (var item in infos)
                {
                    DBContext.StatsicRelation.Add(new StatsicRelation()
                        {
                            StatsicName = statsicName,
                            GroupName = item
                        });
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
