using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Infrastructure.Logging;
using YYQERP.Services.Views;


namespace YYQERP.Controllers
{
    [MyAuthorize]
    public abstract class BaseController : Controller
    {


        public string LoginUserName { get { return this.HttpContext.User.Identity.Name; } }
       

        public IList<string> GetUserOpers()
        {
            IList<string> Opers = new List<string>();
            var userPer = this.HttpContext.Session["UserRolesMenus"] as UserRolePermission;
            if (userPer != null)
            {
                var rolename = userPer.RoleName.ToLower();
                if (rolename == "admin" || rolename == "boss")
                {
                    Opers = userPer.AllOperCodes;
                }
                else
                {
                    var menuCode = Request["menucode"];
                    foreach (var item in userPer.UserMenus)
                    {
                        if (item.Code == menuCode)
                        {
                            Opers = item.Opers;
                            break;
                        }
                    }

                    //  string url = (controllerName + "/" + actionName).ToLower();
                }
            }
            return Opers;
        }



        public T GetInfoByStream<T>() where T : class ,new()
        {
            var sr = new System.IO.StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            var newInfo = JsonHelper.DeserializeObj<T>(stream);
            return newInfo;
        }

        public string UploadFile(string upType, out string msg)
        {
            msg = "";
            string newFilePath = "";

            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase uploadFile = Request.Files[i];
                    var extArr = GetALLowUploadPicType();
                    string fileExt = System.IO.Path.GetExtension(uploadFile.FileName).ToLower();
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(uploadFile.FileName);
                    if (extArr.Contains(fileExt))
                    {
                        float fileSize = (float)uploadFile.ContentLength / 1024 / 1024;
                        if (FileHelper.CheckFileLength(fileSize, out msg))
                        {
                            var path = "/Upfiles/" + upType + "/";
                            var ServerPath = Server.MapPath(path);
                            fileName += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileExt;
                            if (!System.IO.Directory.Exists(ServerPath))
                            {
                                System.IO.Directory.CreateDirectory(ServerPath);
                            }

                            uploadFile.SaveAs(ServerPath + fileName);
                            newFilePath = ServerPath + fileName;
                        }
                    }
                    else
                    {
                        msg = "上传文件类型错误";
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().Log("上传文件错误:\r\n" + ex.ToString());
                msg = ex.Message;
            }
            return newFilePath;
        }

        //public void ExportExcel()
        //{
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.Charset = "utf-8";
        //    Response.ContentEncoding = System.Text.Encoding.UTF8;
        //    Response.AppendHeader("content-disposition", "attachment;filename=\"" + HttpUtility.HtmlEncode(Request["txtName"] ?? DateTime.Now.ToString("yyyyMMdd")) + ".xls\"");
        //    Response.ContentType = "Application/ms-excel";
        //    Response.Write("<html>\n<head>\n");
        //    Response.Write("<style type=\"text/css\">\n.pb{font-size:13px;border-collapse:collapse;} " +
        //                   "\n.pb th{font-weight:bold;text-align:center;border:0.5pt solid windowtext;padding:2px;} " +
        //                   "\n.pb td{border:0.5pt solid windowtext;padding:2px;}\n</style>\n</head>\n");
        //    Response.Write("<body>\n" + Request["txtContent"] + "\n</body>\n</html>");
        //    Response.Flush();
        //    Response.End();   
        //}

        public FileResult Export<T>(IList<T> list, string Columns, string ColumnTitles, string FileName)
        {
            string[] columns = Server.UrlDecode(Columns).Split(',');
            string[] columnTitles = Server.UrlDecode(ColumnTitles).Split(',');
            StringBuilder sb = new StringBuilder("<table border='1' cellspacing='0' cellpadding='0'><tr>");
            foreach (var item in columnTitles)
            {
                sb.AppendFormat("<th>{0}</th>", item);
            }
            sb.Append("</tr>");
            var dataTable = list.ToDataTable<T>(columns);
            foreach (DataRow dr in dataTable.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn column in dataTable.Columns)
                {
                    sb.AppendFormat("<td>{0}</td>", dr[column].ToString());
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");


            byte[] fileContents = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileContents, "application/vnd.ms-excel", FileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

            //第二种:使用FileStreamResult
            //var fileStream = new MemoryStream(fileContents);
            //return File(fileStream, "application/ms-excel", "fileStream.xls");

            //第三种:使用FilePathResult
            //服务器上首先必须要有这个Excel文件,然会通过Server.MapPath获取路径返回.
            //    var fileName = Server.MapPath("~/Files/fileName.xls");
            //  return File(fileName, "application/ms-excel", "fileName.xls");
        }


        private string GetALLowUploadPicType()
        {
            return System.Configuration.ConfigurationManager.AppSettings["UploadFileType"];
        }
    }


}