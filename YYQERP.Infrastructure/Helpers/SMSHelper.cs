using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace YYQERP.Infrastructure.Helpers
{
    public class SMSHelper
    {
        string spID = ConfigurationManager.AppSettings["SpID"];
        string loginName = ConfigurationManager.AppSettings["LoginName"];
        string password = ConfigurationManager.AppSettings["Password"];
        public SMSHelper()
        {

        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ReturnSMSer Send(string phone, string content)
        {
            var result = new ReturnSMSer { Desp = "", Status = 0 };

            try
            {
                if (string.IsNullOrEmpty(phone))
                {
                    return new ReturnSMSer { Desp = "发送号码不能为空", Status = -1 };
                }

                if (string.IsNullOrEmpty(content))
                {
                    return new ReturnSMSer { Desp = "短信内容不能为空", Status = -1 };
                }
                string addr = ConfigurationManager.AppSettings["SMSSendAddr"];
                content = HttpContext.Current.Server.UrlEncode(content);
                string param = string.Format("account={0}&pswd={1}&mobile={2}&msg={3}&needstatus=false&product={4}", loginName, password, phone, content, spID);
                string strReturn = HttpInterface(addr, param);
                if (!string.IsNullOrEmpty(strReturn) && strReturn.Split(',').Length >= 2)
                {
                    var resultFlag = Convert.ToInt32(strReturn.Split(',')[1]);
                    if (resultFlag == (int)SMSStatus.Sucess)
                    {
                        return new ReturnSMSer { Desp = "", Status = resultFlag };
                    }
                    else
                    {
                        var desp = StringHelper.GetEnumDescription((SMSStatus)resultFlag);
                        return new ReturnSMSer { Desp = desp, Status = resultFlag };
                    }
                }
                else
                {
                    return new ReturnSMSer { Desp = "发送错误", Status = -1 };
                }
                // result = StringToXML(strReturn);


            }
            catch (Exception ex)
            {
                result.Status = -1;
                result.Desp = ex.Message;
            }

            return result;
        }

        ///// <summary>
        ///// 发送短信(MT)
        ///// </summary>
        ///// <param name="numberList">发送号码</param>
        ///// <param name="content">短信内容</param>
        ///// <returns></returns>
        //public string Send(List<String> numberList, string content)
        //{
        //    string result = "";
        //    if (numberList != null && numberList.Count > 0)
        //    {
        //        string numbers = string.Join(",", numberList);
        //        if (numbers.Length > 0)
        //        {
        //            result = Send(numbers, content);
        //        }
        //        else
        //        {
        //            result = StringToXML("result=0&description=发送号码不能为空");
        //        }
        //    }
        //    else
        //    {
        //        result = StringToXML("result=0&description=发送号码不能为空");
        //    }
        //    return result;
        //}

        /// <summary>
        /// 接收短信(主动)
        /// </summary>
        /// <returns></returns>
        public string Reply()
        {
            string result = "";
            string addr = ConfigurationManager.AppSettings["SMSReplyAddr"];
            try
            {
                string param = string.Format("spId={0}&loginName={1}&password={2}", spID, loginName, password);
                string strReturn = HttpInterface(addr, param);
                string xml = StringToXML(strReturn);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                string reply = getNodeValue(doc, "replys");
                if (reply != "")
                {
                    reply = ReplyToXML(reply);
                    SetNodeValue(doc, "replys", reply);
                }
                result = doc.InnerXml;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 查询状态报告(主动)
        /// </summary>
        /// <returns></returns>
        public string Report()
        {
            string result = "";
            string addr = ConfigurationManager.AppSettings["SMSReportAddr"];
            try
            {
                string param = string.Format("spId={0}&loginName={1}&password={2}", spID, loginName, password);
                string strReturn = HttpInterface(addr, param);
                string xml = StringToXML(strReturn);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                string report = getNodeValue(doc, "reports");
                if (report != "")
                {
                    report = ReportToXML(report);
                    SetNodeValue(doc, "reports", report);
                }
                result = doc.InnerXml;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <returns></returns>
        public string SearchNumber()
        {
            string result = "";
            string addr = ConfigurationManager.AppSettings["SMSSearchNumberAddr"];
            try
            {
                string param = string.Format("spId={0}&loginName={1}&password={2}", spID, loginName, password);
                string strReturn = HttpInterface(addr, param);
                result = StringToXML(strReturn);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 调用http服务
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string HttpInterface(string url, string param)
        {
            string result = "";
            Encoding encoding = Encoding.GetEncoding("GBK");
            byte[] postBytes = encoding.GetBytes(param);
            WebResponse wResponse = null;
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = WebRequestMethods.Http.Post; // Post method 
            wRequest.Timeout = 50000;
            wRequest.ContentType = "application/x-www-form-urlencoded";

            wRequest.ContentLength = postBytes.Length;

            using (Stream reqStream = wRequest.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
            }
            //尝试获得要请求的URL的返回消息
            wResponse = (HttpWebResponse)wRequest.GetResponse();
            if (wResponse != null)
            {
                //获得网络响应流
                using (StreamReader responseReader = new StreamReader(wResponse.GetResponseStream(), encoding))
                {
                    result = responseReader.ReadToEnd();//获得返回流中的内容
                }
                wResponse.Close();//关闭web响应流
            }
            return result;

            //WebResponse wResponse = wRequest.GetResponse();
            //Stream stream = wResponse.GetResponseStream();
            //StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            //string result = reader.ReadToEnd();
            //wResponse.Close();
            //return result;
        }

        /// <summary>
        /// 将返回的字符串转化为xml
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string StringToXML(string result)
        {
            string xml = "<return>";
            string[] returnList = result.Split('&');
            foreach (string s in returnList)
            {
                if (s.Trim() != "")
                {
                    string[] strList = s.Split('=');
                    if (strList.Length != 2)
                    {
                        xml += "<result>" + strList[0] + "</result>";
                    }
                    else
                    {
                        xml += "<" + strList[0] + ">" + strList[1] + "</" + strList[0] + ">";
                    }
                }
            }
            xml += "</return>";
            return xml;
        }

        /// <summary>
        /// 将接收短信返回的replys节点内容转换为xml
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string ReplyToXML(string result)
        {
            string xml = "";
            string[] returnList = result.Split(';');
            foreach (string s in returnList)
            {
                if (s.Trim() != "")
                {
                    string[] strList = s.Split(',');
                    xml += "<reply>";
                    xml += "<callmdn>" + strList[0] + "</callmdn>";
                    xml += "<mdn>" + strList[1] + "</mdn>";
                    xml += "<content>" + strList[2] + "</content>";
                    xml += "<reply_time>" + strList[3] + "</reply_time>";
                    xml += "<id>" + strList[4] + "</id>";
                    xml += "</reply>";
                }
            }
            return xml;
        }

        /// <summary>
        /// 将状态报告返回的reports节点内容转换为xml
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string ReportToXML(string result)
        {
            string xml = "";
            string[] returnList = result.Split(';');
            foreach (string s in returnList)
            {
                if (s.Trim() != "")
                {
                    string[] strList = s.Split(',');
                    xml += "<report>";
                    xml += "<smsId>" + strList[0] + "</smsId>";
                    xml += "<mdn>" + strList[1] + "</mdn>";
                    xml += "<status>" + strList[2] + "</status>";
                    xml += "<statdesc>" + strList[3] + "</statdesc>";
                    xml += "<arrive_time>" + strList[4] + "</arrive_time>";
                    xml += "</report>";

                }
            }
            return xml;
        }

        /// <summary>
        /// 获得节点值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string getNodeValue(XmlDocument doc, string name)
        {
            try
            {
                XmlNode node_s = doc.GetElementsByTagName(name)[0];
                string value = node_s.InnerXml;
                return value;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetNodeValue(XmlDocument doc, string name, string value)
        {
            try
            {
                XmlNode node_s = doc.GetElementsByTagName(name)[0];
                node_s.InnerXml = value;
            }
            catch
            {
            }
        }
    }

     public class SMSSender
    {
        public string account { get; set; }
        public string pswd { get; set; }
        public string mobile { get; set; }
        public string msg { get; set; }
        public bool needstatus { get; set; }
        public string product { get; set; }
       
    }

    public enum SMSStatus
    {
        [Description("提交成功")]
        Sucess = 0,
        [Description("无此用户")]
        status2 = 101,
        [Description("密码错")]
        status3 = 102,
        [Description("提交过快（提交速度超过流速限制）")]
        status4 = 103,
        [Description("系统忙（因平台侧原因，暂时无法处理提交的短信）")]
        status5 = 104,
        [Description("敏感短信（短信内容包含敏感词）")]
        status6 = 105,
        [Description("消息长度错（>536或<=0）")]
        status7 = 106,
        [Description("包含错误的手机号码")]
        status8 = 107,
        [Description("手机号码个数错（群发>50000或<=0;单发>200或<=0）")]
        status9 = 108,
        [Description("无发送额度（该用户可用短信数已使用完）")]
        status10 = 109,
        [Description("不在发送时间内")]
        status11 = 110,
        [Description("超出该账户当月发送额度限制")]
        status12 = 111,
        [Description("无此产品，用户没有订购该产品")]
        status13 = 112,
        [Description("extno格式错（非数字或者长度不对）")]
        status14 = 113,
        [Description("自动审核驳回")]
        status15 = 115,
        [Description("签名不合法，未带签名（用户必须带签名的前提下）")]
        status16 = 116,
        [Description("IP地址认证错,请求调用的IP地址不是系统登记的IP地址")]
        status17 = 117,
        [Description("用户没有相应的发送权限")]
        status18 = 118,
        [Description("用户已过期")]
        status19 = 119,
        [Description("内容不在白名单中")]
        status20 = 120

    }


    public class ReturnSMSer
    {
        public int Status { get; set; }

        public string Desp { get; set; }
    }
}
