
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

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
            string file = service.Create(absoluFilePath);

            var fileStream = new FileStream(file, FileMode.Open);

            return File(fileStream, "application/octet-stream", Server.UrlEncode(file));
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
                    string file = service.Create(absoluFilePath);

                    var msg = "";
                    var sendMail = new MailLib.MailHelper("smtp.163.com", 25, "zhangwxyc@163.com", "qwerty123", System.Configuration.ConfigurationManager.AppSettings["toUsers"].Split(','), "how are you", "body is xxxx", false);
                    sendMail.IsSendAttachments = true;
                    sendMail.Attachments = new string[] { file };
                    var s = sendMail.Send(out msg);
                    if (s==MailLib.MailHelper.SendStatus.Success)
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

            return Json(rInfo,JsonRequestBehavior.AllowGet);
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