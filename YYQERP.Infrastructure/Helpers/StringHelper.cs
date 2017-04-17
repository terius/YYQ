﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Helpers
{
    public class StringHelper
    {
        public static string TenToTwo(int value)
        {
            string j = Convert.ToString(value, 2);//j就是转换后的二进制了！！
            return j.PadLeft(14, '0');
        }

        public static int TwoToTen(string value)
        {
            int j = Convert.ToInt32(value, 2);
            return j;
        }

        /// <summary>
        /// 数字的安全转换
        /// </summary>
        /// <param name="oInt"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static int SafeGetIntFromObj(object oInt, int defaultVal = 0)
        {
            int iTemp = defaultVal;
            if (oInt == null)
                return defaultVal;
            return int.TryParse(oInt.ToString(), out iTemp) ? iTemp : defaultVal;
        }


        /// <summary>
        /// Double数字的安全转换
        /// </summary>
        /// <param name="oInt"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static double SafeGetDoubleFromObj(object oInt, double defaultVal = 0)
        {
            double iTemp = 0;
            if (oInt == null)
                return defaultVal;
            return double.TryParse(oInt.ToString(), out iTemp) ? iTemp : defaultVal;
        }


        /// <summary>
        /// 安全时间转换
        /// </summary>
        /// <param name="oDateTime"></param>
        /// <returns></returns>
        public static DateTime SafeGetDateTimeFromObj(object oDateTime, string defaultTime = null)
        {
            DateTime dftime = string.IsNullOrEmpty(defaultTime) ? DateTime.MinValue : Convert.ToDateTime(defaultTime);
            if (oDateTime == null)
            {
                return dftime;
            }
            DateTime t = dftime;
            if (DateTime.TryParse(oDateTime.ToString(), out t))
            {
                return t;
            }
            else
            {
                return dftime;
            }


        }

        public static DateTime? SafeGetNullAbleDateTimeFromObj(object oDateTime)
        {
            if (oDateTime == null)
            {
                return null;
            }
            DateTime t = DateTime.Now;
            if (DateTime.TryParse(oDateTime.ToString(), out t))
            {
                return t;
            }
            else
            {
                return null;
            }


        }
        public static decimal? SafeGetNullAbleDecimalFromObj(object ob)
        {
            if (ob == null)
            {
                return null;
            }
            decimal t = 0;
            if (decimal.TryParse(ob.ToString(), out t))
            {
                return t;
            }
            else
            {
                return null;
            }


        }

        public static decimal SafeGetDecimalFromObj(object ob)
        {
            decimal t = 0;
            if (decimal.TryParse(ob.ToString(), out t))
            {
                return t;
            }
            else
            {
                return 0;
            }


        }



        /// <summary>
        /// 比较两个byte[]数组是否相等
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool ByteEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;

            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 根据枚举得到描述信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                return "未知";
            }
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return "未知";
            }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }


        /// <summary>
        /// 获取一天上下午晚上描述
        /// </summary>
        /// <param name="daypart"></param>
        /// <returns></returns>
        public static string GetDayPartText(int daypart)
        {
            switch (daypart)
            {
                case 1:
                    return "上午";
                case 2:
                    return "下午";
                case 3:
                    return "晚上";
                default:
                    break;
            }
            return "";
        }


        public static string NumToChineseNum(int num)
        {
            string chineseNum = "";
            switch (num)
            {
                case 0:
                    chineseNum = "零";
                    break;
                case 1:
                    chineseNum = "一";
                    break;
                case 2:
                    chineseNum = "二";
                    break;
                case 3:
                    chineseNum = "三";
                    break;
                case 4:
                    chineseNum = "四";
                    break;
                case 5:
                    chineseNum = "五";
                    break;
                case 6:
                    chineseNum = "六";
                    break;
                case 7:
                    chineseNum = "七";
                    break;
                case 8:
                    chineseNum = "八";
                    break;
                case 9:
                    chineseNum = "九";
                    break;
                case 10:
                    chineseNum = "十";
                    break;
                default:
                    break;
            }
            return chineseNum;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }


        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="Id">身份证号码</param>
        /// <returns>验证成功为True，否则为False</returns>
        public static bool CheckIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证15位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (!DateTime.TryParse(birth, out time))
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }


        /// <summary>
        /// 验证18位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }


        public static DateTime GetBirthdayFromIdCard(string idcard)
        {
            DateTime dt = Convert.ToDateTime("1980-01-01");
            string birth = idcard.Length > 14 ? idcard.Substring(6, 8).Insert(6, "-").Insert(4, "-") : "1980-01-01";
            if (DateTime.TryParse(birth, out dt))
            {
                return dt;
            }
            return dt;
        }


        public static bool GetSexFromIdCard(string idcard)
        {
            string sexFlag = "1";
            if (idcard.Length == 15)
            {
                sexFlag = idcard.Substring(14, 1);
            }
            else if (idcard.Length == 18)
            {
                sexFlag = idcard.Substring(16, 1);
            }
            bool sex = Convert.ToInt32(sexFlag) % 2 == 1;
            return sex;
        }


        /// <summary>
        /// 数组去除重复数据和空字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string[] DelDupAndEmpty(string[] s)
        {

            HashSet<string> hs = new HashSet<string>(s); //此时已经去掉重复的数据保存在hashset中
            hs.Remove("");//去除空字符串
            String[] rid = new string[hs.Count];
            hs.CopyTo(rid);
            return rid;
        }





        //public static bool CheckDateIsInPlan(DateTime selectDate, int PlanValue)
        //{
        //    if (PlanValue <= 0)
        //    {
        //        return false;
        //    }
        //    if (selectDate <DateTime.Now)
        //    {
        //        return false;
        //    }
        //    string planStr = TenToTwo(PlanValue);

        //    DayOfWeek dow = selectDate.DayOfWeek;
        //    int timeWeek = (int)dow == 0 ? 7 : (int)dow;
        //    bool timeIsShangWu = selectDate.Hour < 12 ? true : false;
        //    int idx = 0;
        //    foreach (char c in planStr)
        //    {
        //        if (c.ToString() == "1")
        //        {
        //            int week = idx / 2 + 1;
        //            bool isShangWu = idx % 2 == 0 ? true : false;
        //            if (timeWeek == week)
        //            {
        //                if (timeIsShangWu == isShangWu)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        idx++;
        //    }

        //    return false;
        //}


        //public static bool CheckPlan(string bigStr, string smallStr)
        //{
        //    if (bigStr == "00000000000000" || string.IsNullOrEmpty(bigStr))
        //    {
        //        return false;
        //    }
        //    if (smallStr == "00000000000000" || string.IsNullOrEmpty(smallStr))
        //    {
        //        return true;
        //    }

        //    if (bigStr.Length != smallStr.Length)
        //    {
        //        return false;
        //    }
        //    for (int i = 0; i < smallStr.Length; i++)
        //    {
        //        if (smallStr[i].ToString() == "1")
        //        {
        //            if (bigStr[i] == smallStr[i])
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}



        #region RSA加解密
        //加密算法  
        public static string RSAEncrypt(string encryptString)
        {


            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "teriusyouareveryniubihahaha";

            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(csp);
            byte[] encryptBytes = RSAProvider.Encrypt(ASCIIEncoding.ASCII.GetBytes(encryptString), true);
            string str = "";
            foreach (byte b in encryptBytes)
            {
                str = str + string.Format("{0:x2}", b);
            }
            return str;
        }


        //解密算法  
        public static string RSADecrypt(string decryptString)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = "teriusyouareveryniubihahaha";
            RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider(csp);
            int length = (decryptString.Length / 2);
            byte[] decryptBytes = new byte[length];
            for (int index = 0; index < length; index++)
            {
                string substring = decryptString.Substring(index * 2, 2);
                decryptBytes[index] = Convert.ToByte(substring, 16);
            }
            decryptBytes = RSAProvider.Decrypt(decryptBytes, true);
            return ASCIIEncoding.ASCII.GetString(decryptBytes);
        }

        #endregion


        public static string Sha256(string plainText)
        {
            SHA256Managed _sha256 = new SHA256Managed();
            byte[] _cipherText = _sha256.ComputeHash(Encoding.Default.GetBytes(plainText));
            return Convert.ToBase64String(_cipherText);
        }


        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        public static string CmycurD(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

    }
}
