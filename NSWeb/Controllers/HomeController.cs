
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using StatsisLib;

namespace NSWeb.Controllers
{
    public class HomeController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();

        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Main()
        {
            //ViewData["files"] = DBContext.UploadInfo.Where(x => x.IsDel == 0).OrderByDescending(x => x.CreateTime).ToList();
            return View("Main");
        }
        [ActionName("fileList")]
        public ContentResult GetFileList()
        {
            var infos = DBContext.UploadInfo.Where(x => x.IsDel == 0).OrderByDescending(x => x.CreateTime).ToList();
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

            return Content(JsonConvert.SerializeObject(infos, Formatting.Indented, timeConverter));
            // return Json(infos, JsonRequestBehavior.AllowGet);
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
                    foreach (string file in Request.Files)
                    {

                        HttpPostedFileBase item = Request.Files[file] as HttpPostedFileBase;

                        if (item != null && item.ContentLength > 0)
                        {
                            string saveName = string.Format("{0}_{1}", DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace("/", "_").Replace(" ", "_"), item.FileName);
                            DBContext.UploadInfo.Add(new DataService.UploadInfo()
                                {
                                    CreateTime = DateTime.Now,
                                    Name = item.FileName,
                                    SaveName = saveName,
                                    FromIP = Request.UserHostAddress,
                                    FromUser = Request.UserHostName,
                                    IsDel = 0
                                });
                            item.SaveAs(GetPath(saveName));
                            //using (BinaryReader br = new BinaryReader(item.InputStream))
                            //{
                            //    byte[] datas = br.ReadBytes((int)item.InputStream.Length);
                            //    System.IO.File.WriteAllBytes(GetPath(saveName), datas);
                            //}
                            DBContext.SaveChanges();
                            rInfo.IsSuccess = true;
                        }
                    }
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
                if (files != null && files.Count > 0)
                {
                    foreach (string file in Request.Files)
                    {

                        HttpPostedFileBase item = Request.Files[file] as HttpPostedFileBase;

                        if (item != null && item.ContentLength > 0)
                        {
                            //string saveName = string.Format("{0}_{1}", DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace("/", "_").Replace(" ", "_"), item.FileName);
                            //DBContext.UploadInfo.Add(new DataService.UploadInfo()
                            //    {
                            //        CreateTime = DateTime.Now,
                            //        Name = item.FileName,
                            //        SaveName = saveName,
                            //        FromIP = Request.UserHostAddress,
                            //        FromUser = Request.UserHostName,
                            //        IsDel = 0
                            //    });
                            //item.SaveAs(GetPath(saveName));
                            //using (BinaryReader br = new BinaryReader(item.InputStream))
                            //{
                            //    byte[] datas = br.ReadBytes((int)item.InputStream.Length);
                            //    System.IO.File.WriteAllBytes(GetPath(saveName), datas);
                            //}
                            DBContext.SaveChanges();
                            rInfo.IsSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rInfo.Message = ex.Message;
                Json(rInfo);
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

        [ActionName("down_src_trim")]
        public FileStreamResult DownFileTrim(int id)
        {
            var info = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
            if (info == null)
            {
                return new FileStreamResult(null, "error");
            }
            string absoluFilePath = GetPath(info.SaveName);
            var totalInfos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(absoluFilePath, 0, 1));

            List<BaseDataInfo> dataInfos = new List<BaseDataInfo>();

            string tcName = Path.GetFileNameWithoutExtension(info.Name) + "_tc.xls";
            var tcFile = DBContext.UploadInfo.FirstOrDefault(x => x.Name == tcName);
            if (tcFile != null)
            {
                dataInfos = StatsisLib.Common.DTToList<BaseDataInfo>(NPOIHelper.ImportExceltoDt(GetPath(tcFile.SaveName)));
                #region d


                totalInfos = totalInfos.Where(x => !string.IsNullOrWhiteSpace(x.工号)).ToList();
                foreach (var item in totalInfos)
                {
                    var fInfo = dataInfos.FirstOrDefault(x => x.工号 == item.工号);
                    if (fInfo != null)
                    {
                        item.录音抽检数 -= fInfo.录音抽检数;
                        item.中度服务瑕疵量 -= fInfo.中度服务瑕疵量;
                        item.重大服务失误量 -= fInfo.重大服务失误量;
                        item.总接听量 -= fInfo.总接听量;
                        item.满意 -= fInfo.满意;
                        item.不满意 -= fInfo.不满意;
                        item.一般 -= fInfo.一般;
                        item.总量 -= fInfo.总量;
                        item.通过量 -= fInfo.通过量;
                    }
                }

                #endregion
            }

            var cols = System.Configuration.ConfigurationManager.AppSettings["MainT"].Split(',').ToList();
            var dt1 = StatsisLib.Common.ListToDataTable(totalInfos, cols);
            dt1.TableName = "1";
            var dt2 = StatsisLib.Common.ListToDataTable(dataInfos, cols);
            dt2.TableName = "2";

            string tmpFilePath = GetPath(Path.GetFileNameWithoutExtension(info.SaveName) + "_tc_r.xls");
            if (System.IO.File.Exists(tmpFilePath))
            {
                System.IO.File.Delete(tmpFilePath);
            }
            NPOIHelper.ExportSimple(
                new List<System.Data.DataTable>() 
                {
                   dt1,
                   dt2
                },
                tmpFilePath);



            var fileStream = new FileStream(tmpFilePath, FileMode.Open);
            return File(fileStream, "application/octet-stream", Server.UrlEncode(info.SaveName));
        }


        [ActionName("down_Anaysle")]
        public FileStreamResult DownAnaysleFile(int id)
        {
            var info = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
            if (info == null)
            {
                return new FileStreamResult(null, "error");
            }
            string absoluFilePath = GetPath(info.SaveName);

            UnionLib.AnaysleService service = new UnionLib.AnaysleService();
            string file = service.Create(absoluFilePath,GetAssistParams(id));

            var fileStream = new FileStream(file, FileMode.Open);

            return File(fileStream, "application/octet-stream", Server.UrlEncode(file));
        }

        private Dictionary<string, object> GetAssistParams(int id)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            var currentInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
            if (currentInfo != null)
            {
               // list.Add("CurrentInfo", currentInfo);
                string trimFileName = Path.GetFileNameWithoutExtension(currentInfo.Name) + "_tc.xls";
                var trimInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Name == trimFileName && x.IsDel.Value==0);

                if (trimInfo!=null)
                {
                    list.Add("tc", GetPath(trimInfo.SaveName));
                }

                string mydFileName = Path.GetFileNameWithoutExtension(currentInfo.Name) + "_myd.xls";
                var mydInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Name == mydFileName && x.IsDel.Value == 0);

                if (mydInfo != null)
                {
                    list.Add("myd", GetPath(mydInfo.SaveName));
                }
                string tslFileName = Path.GetFileNameWithoutExtension(currentInfo.Name) + "_tsl.xls";
                var tslInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Name == tslFileName && x.IsDel.Value == 0);

                if (tslInfo != null)
                {
                    list.Add("tsl", GetPath(tslInfo.SaveName));
                }
            }

            return list;
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

        [ActionName("email")]
        public JsonResult SendTo(int id)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();

            try
            {

                var currentInfo = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);

                if (currentInfo != null)
                {
                    string absoluFilePath = GetPath(currentInfo.SaveName);

                    UnionLib.AnaysleService service = new UnionLib.AnaysleService();
                    string file = service.Create(absoluFilePath,GetAssistParams(id));

                    var msg = "";
                    var sendMail = new MailLib.MailHelper("smtp.163.com", 25, "zhangwxyc@163.com", "qwerty123", System.Configuration.ConfigurationManager.AppSettings["toUsers"].Split(','), "how are you", "body is xxxx", false);
                    sendMail.IsSendAttachments = true;
                    sendMail.Attachments = new string[] { file };
                    var s = sendMail.Send(out msg);
                    if (s == MailLib.MailHelper.SendStatus.Success)
                    {
                        rInfo.IsSuccess = true;
                    }
                    else
                    {
                        rInfo.Message = msg;
                    }
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

            return Json(rInfo, JsonRequestBehavior.AllowGet);
        }


        private string GetPath(string saveName)
        {
            return Path.Combine(Server.MapPath("~/App_Data/Files/"), saveName);
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