using StatsisLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StatsisLib;
using System.Data;

namespace NSWeb.Controllers
{
    public class HomeController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Main()
        {
            ViewData["files"] = DBContext.UploadInfo.Where(x => x.IsDel == 0).OrderByDescending(x => x.CreateTime).ToList();
            return View("Main");
        }
        [ActionName("upload")]
        public JsonResult UpLoad()
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            try
            {
                var files = Request.Files;
                if (files != null && files.Count > 0)
                {
                    foreach (HttpPostedFile item in files)
                    {
                        string saveName = string.Format("{0}_{1}", DateTime.Now.ToString().Replace("-", "").Replace(":", ""), item.FileName);
                        DBContext.UploadInfo.Add(new DataService.UploadInfo()
                            {
                                CreateTime = DateTime.Now,
                                Name = item.FileName,
                                SaveName = saveName,
                                FromIP = Request.UserHostAddress,
                                FromUser = Request.UserHostName,
                                IsDel = 0
                            });

                        using (BinaryReader br = new BinaryReader(item.InputStream))
                        {
                            byte[] datas = br.ReadBytes((int)item.InputStream.Length);
                            System.IO.File.WriteAllBytes(GetPath(saveName), datas);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(rInfo);
        }

        [ActionName("down_src")]
        public FileStreamResult DownFile(int id)
        {
            var info = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
            if (info == null)
            {
                return new FileStreamResult(null, "error");
            }
            string absoluFilePath = GetPath(info.SaveName);
            var fileStream = new FileStream(absoluFilePath, FileMode.Open);
            return File(fileStream, "application/octet-stream", Server.UrlEncode(info.SaveName));
        }

       // [ActionName("down_Anaysle")]
 //       public FileStreamResult DownFile(int id)
 //       {
 //           var info = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
 //           if (info == null)
 //           {
 //               return new FileStreamResult(null, "error");
 //           }
 //           string absoluFilePath = GetPath(info.SaveName);
           
 //           var dt = NPOIHelper.ImportExceltoDt(absoluFilePath, 0, 0);
 //           var SrcInfos =StatsisLib.Common.DTToList<BaseDataInfo>(dt);
 //           SrcInfos = SrcInfos.Where(x => !string.IsNullOrEmpty(x.姓名)).ToList();
 //           SrcInfos = FilterUsers(SrcInfos);
 //           UpdateDest();
 //           List<DataTable> ds = new List<DataTable>()
 //           {
 //               DataProcess.T1(DestInfos),
 //               DataProcess.T2(DestInfos),
 //               DataProcess.T2_5(DestInfos),

 //               DataProcess.T4(DestInfos),
 //               DataProcess.T3(DestInfos),
 //               DataProcess.T5(DestInfos)
 //           };
 //           int index = 0;
 //           string[] names = Common.GetConfig("T_Names").Split(',');
 //           ds.ForEach(x => x.TableName = names[index++]);
 //           NPOIHelper.ExportSimple(ds, "C:\\1q.xlsx");

 //var fileStream = new FileStream(absoluFilePath, FileMode.Open);

 //           return File(fileStream, "application/octet-stream", Server.UrlEncode(fileName));
 //       }

        private List<StatsisLib.BaseDataInfo> FilterUsers(List<StatsisLib.BaseDataInfo> SrcInfos)
        {
            throw new NotImplementedException();
        }

        [ActionName("d")]
        public JsonResult OpGroup(int id)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();

            try
            {

                var currentInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);

                if (currentInfo != null)
                {
                    currentInfo.IsDel = 1;
                    DBContext.SaveChanges();
                    rInfo.IsSuccess = true;
                }
                else
                {
                    rInfo.Message = string.Format("没有 {0}", id);
                }

            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
            }

            return Json(rInfo);
        }

        private string GetPath(string saveName)
        {
            return Path.Combine(Server.MapPath("~/AppData/Files/"), saveName);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}