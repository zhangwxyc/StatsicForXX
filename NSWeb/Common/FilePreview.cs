using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSWeb.Common
{
    public class FilePreview
    {
        //public static void Priview(System.Web.UI.Page p, string inFilePath, string outDirPath = "")
        //{
        //    Microsoft.Office.Interop.Excel.Application excel = null;
        //    Microsoft.Office.Interop.Excel.Workbook xls = null;
        //    excel = new Microsoft.Office.Interop.Excel.Application();
        //    object missing = Type.Missing;
        //    object trueObject = true;
        //    excel.Visible = false;
        //    excel.DisplayAlerts = false;
        //    string randomName = DateTime.Now.Ticks.ToString();  //output fileName
        //    xls = excel.Workbooks.Open(inFilePath, missing, trueObject, missing,
        //                                missing, missing, missing, missing, missing, missing, missing, missing,
        //                                missing, missing, missing);
        //    //Save Excel to Html
        //    object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
        //    Workbook wsCurrent = xls;//(Workbook)wsEnumerator.Current;
        //    String outputFile = outDirPath + randomName + ".html";
        //    wsCurrent.SaveAs(outputFile, format, missing, missing, missing,
        //                      missing, XlSaveAsAccessMode.xlNoChange, missing,
        //                      missing, missing, missing, missing);
        //    excel.Quit();
        //    //Open generated Html
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = true;
        //    process.StartInfo.FileName = outputFile;
        //    process.Start();
        //}         
    }
}