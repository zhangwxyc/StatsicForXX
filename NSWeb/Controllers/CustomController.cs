using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using StatsisLib;

namespace NSWeb.Controllers
{
    public class CustomController : Controller
    {
        DataService.QHXEntities DBContext = new DataService.QHXEntities();


        public ActionResult Index()
        {
            var infos = DBContext.UploadInfo.Where(x => x.IsDel == 0).OrderByDescending(x => x.CreateTime).ToList();
            ViewData["fileList"] = infos;
            return View();
        }

        [ActionName("f")]
        public JsonResult T1(int id, bool isReset = false)
        {
            var info = DBContext.UploadInfo.FirstOrDefault(x => x.Id == id);
            if (info == null)
            {
                return Json(null, "error");
            }

            string dataPath = GetDataPath(id.ToString());
            List<StatsisLib.BaseDataInfo> infos = null;
            if (isReset || !System.IO.File.Exists(dataPath))
            {
                if (System.IO.File.Exists(dataPath))
                {
                    System.IO.File.Move(dataPath, dataPath + DateTime.Now.ToFileTime() + ".bak");
                }
                string absoluFilePath = GetPath(info.SaveName);
                UnionLib.AnaysleService service = new UnionLib.AnaysleService();
                infos = service.GetSumLineTable(absoluFilePath);
                infos.Serialize(dataPath);
            }
            else
            {
                infos = infos.Deserialize(dataPath);
            }
            infos = infos.OrderByDescending(x => x.通过率).ThenByDescending(x => x.净满意度).ToList();
            return Json(infos);
        }
        private string GetPath(string saveName)
        {
            return Path.Combine(Server.MapPath("~/App_Data/Files/"), saveName);
        }
        private string GetDataPath(string id)
        {
            return string.Format(Server.MapPath("~/App_Data/Files/ansyle_{0}.xml"), id);
        }
        private string GetDataPath_excel(string id)
        {
            return string.Format(Server.MapPath("~/App_Data/Files/ansyle_{0}.xls"), id);
        }

        [ActionName("down_Anaysle")]
        public FileStreamResult DownAnaysleFile(int id)
        {

            string dataPath = GetDataPath(id.ToString());
            List<StatsisLib.BaseDataInfo> infos = null;
            if (!System.IO.File.Exists(dataPath))
            {
                return null;
            }
            else
            {
                infos = infos.Deserialize(dataPath);
            }
            UnionLib.AnaysleService service = new UnionLib.AnaysleService();
            var tb = DataProcess.T1_0(infos);
            tb.TableName = "调整后的";
            string path = GetDataPath_excel(id.ToString());
            NPOIHelper.ExportSimple(new List<System.Data.DataTable>() { tb }, path);

            var fileStream = new FileStream(path, FileMode.Open);

            return File(fileStream, "application/octet-stream", Server.UrlEncode(path));
        }

        public class RefreshParam
        {
            public int Id { get; set; }
            public string Datas { get; set; }
        }

        [ActionName("Refresh")]
        public JsonResult Refresh(RefreshParam info)
        {
            Common.ResultInfo rInfo = new Common.ResultInfo();
            if (info == null)
            {
                rInfo.Message = "参数不正确";
                return Json(rInfo);
            }

            try
            {
                string[] lines = info.Datas.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string dataPath = GetDataPath(info.Id.ToString());
                List<StatsisLib.BaseDataInfo> infos = null;
                infos = infos.Deserialize(dataPath);
                var lineInfos = lines.Select(x => new { GroupName = x.Split('|')[0], PName = x.Split('|')[1], PValue = x.Split('|')[2] }).GroupBy(x => x.GroupName).ToDictionary(x => x.Key, x => x.ToList());

                foreach (var line in lineInfos)
                {
                    var lineInfo = infos.FirstOrDefault(x => x.技能组 == line.Key);
                    if (lineInfo == null)
                    {
                        continue;
                    }
                    foreach (var item in line.Value)
                    {
                        int intValue = 0;
                        if (int.TryParse(item.PValue, out intValue))
                        {
                            var pInfo = lineInfo.GetType().GetProperty(item.PName);
                            if (pInfo != null)
                            {
                                pInfo.SetValue(lineInfo, intValue, null);
                            }
                        }
                    }
                }

               // DataProcess.ComputeV2(infos);
                DataProcess.Compute(infos);
                infos = infos.OrderByDescending(x => x.通过率).ThenByDescending(x => x.净满意度).ToList();

                infos.Serialize(dataPath);

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
