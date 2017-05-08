using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;


namespace YYQERP.Infrastructure.Helpers
{
    public class ExcelHelper
    {
        //  static HSSFWorkbook hssfworkbook;

        //     static XSSFWorkbook xssfworkbook;


        public static DataTable GetData(string filePath)
        {
            IWorkbook wk = null;
            bool isHss = false;
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (filePath.ToLower().EndsWith(".xls"))
                    {
                        wk = new HSSFWorkbook(file);
                        isHss = true;
                    }
                    else
                    {

                        wk = new XSSFWorkbook(file);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            DataTable dt = new DataTable();

            NPOI.SS.UserModel.ISheet sheet = wk.GetSheetAt(0);
            try
            {


                //获取标题
                var row1 = sheet.GetRow(0);//获取第一行即标头  
                int cellCount = row1.LastCellNum; //第一行的列数  
                string excelColName;
                for (int j = 0; j < cellCount; j++)
                {
                    excelColName = row1.GetCell(j).StringCellValue.ToUpper().Trim();
                    // if (!string.IsNullOrEmpty(excelColName))
                    //  {

                    DataColumn column = new DataColumn(excelColName);

                    dt.Columns.Add(column);
                    // }

                    // dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                }
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                rows.MoveNext();
                while (rows.MoveNext())
                {
                    IRow row = null;
                    if (isHss)
                    {
                        row = (HSSFRow)rows.Current;
                    }
                    else
                    {
                        row = (XSSFRow)rows.Current;
                    }
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < cellCount; i++)
                    {

                        ICell cell = row.GetCell(i);

                        if (cell == null || cell.ToString().ToUpper() == "NULL")
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                dr[i] = cell.DateCellValue;
                            }
                            else if (cell.CellType == CellType.Formula)
                            {
                                dr[i] = cell.NumericCellValue.ToString();
                            }
                            else
                            {
                                dr[i] = cell.ToString().Trim();
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                wk = null;
                sheet = null;
            }
            return dt;
        }


        public static DataTable GetSaleReportData(string filePath)
        {
            IWorkbook wk = null;
            bool isHss = false;
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (filePath.ToLower().EndsWith(".xls"))
                    {
                        wk = new HSSFWorkbook(file);
                        isHss = true;
                    }
                    else
                    {
                        //XSSFWorkbook System.OutOfMemoryException
                        wk = new XSSFWorkbook(file);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            DataTable dt = new DataTable();

            ISheet sheet = null;
            try
            {

                int sheetCount = wk.NumberOfSheets;
                for (int k = 0; k < sheetCount; k++)
                {
                    sheet = wk.GetSheetAt(k);
                    if (dt.Columns.Count == 0)
                    {
                        //获取标题
                        var row1 = sheet.GetRow(2);//获取第一行即标头  
                        int cellCount = row1.LastCellNum; //第一行的列数  
                        string excelColName;
                        for (int j = 0; j < cellCount; j++)
                        {
                            excelColName = row1.GetCell(j).StringCellValue.ToUpper().Trim();
                            if (!string.IsNullOrEmpty(excelColName))
                            {
                                DataColumn column = new DataColumn(excelColName);
                                dt.Columns.Add(column);
                            }
                        }
                    }
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                    rows.MoveNext();
                    rows.MoveNext();
                    rows.MoveNext();
                    while (rows.MoveNext())
                    {
                        IRow row = null;
                        if (isHss)
                        {
                            row = (HSSFRow)rows.Current;
                        }
                        else
                        {
                            row = (XSSFRow)rows.Current;
                        }

                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (i == 13)
                            {

                            }
                            ICell cell = row.GetCell(i);

                            if (cell == null || cell.ToString().ToUpper() == "NULL")
                            {
                                dr[i] = null;
                            }
                            else
                            {
                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dr[i] = cell.DateCellValue;
                                }
                                else if (cell.CellType == CellType.Formula)
                                {
                                    dr[i] = cell.NumericCellValue.ToString();
                                }
                                else
                                {
                                    dr[i] = cell.ToString().Trim();
                                }
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                wk = null;
                sheet = null;
            }
            return dt;
        }



        public static void ExportInvoice(string modelExlPath, string downExlPath, DeliveryForPrint info)
        {
            XSSFWorkbook workbook;
            //读入刚复制的要导出的excel文件
            using (FileStream file = new FileStream(modelExlPath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
                file.Close();
            }
            ISheet sheet1 = workbook.GetSheetAt(0);
            //粗体字体
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            //  font.FontHeightInPoints = 20;
            font.Boldweight = 700;

            ICell cell = sheet1.GetRow(0).GetCell(7);
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.SetCellValue("NO." + info.SerialNo);

            //  ICellStyle cellstyle = workbook.CreateCellStyle();
            //客户
            cell = sheet1.GetRow(1).GetCell(1);
            //  cell.CellStyle = cellstyle;
            // cell.CellStyle.Alignment = HorizontalAlignment.Center;
            IRichTextString rts = new XSSFRichTextString(cell.StringCellValue + info.Customer);
            rts.ApplyFont(0, 3, font);
            cell.SetCellValue(rts);
            //订单号
            cell = sheet1.GetRow(1).GetCell(3);
            rts = new XSSFRichTextString(cell.StringCellValue + info.OrderNo);
            rts.ApplyFont(0, 5, font);
            cell.SetCellValue(rts);
            //订单日期
            cell = sheet1.GetRow(1).GetCell(6);
            cell.CellStyle.SetFont(font);
            cell.CellStyle.Alignment = HorizontalAlignment.Right;
            cell.SetCellValue(info.OrderDate);

            int index = 3;
            foreach (var item in info.Details)
            {
                cell = sheet1.GetRow(index).GetCell(1);
                cell.SetCellValue(item.Model);
                cell = sheet1.GetRow(index).GetCell(3);
                cell.SetCellValue(item.Quantity);
                cell = sheet1.GetRow(index).GetCell(4);
                cell.SetCellValue(item.Unit);
                cell = sheet1.GetRow(index).GetCell(5);
                cell.SetCellValue(item.Price);
                cell = sheet1.GetRow(index).GetCell(6);
                cell.SetCellValue(item.TotalPrice);
                cell = sheet1.GetRow(index).GetCell(7);
                cell.SetCellValue(item.Remark);
                index++;
            }
            cell = sheet1.GetRow(10).GetCell(3);
            cell.SetCellValue(info.TotalAmountUp);
            cell = sheet1.GetRow(10).GetCell(7);
            cell.SetCellValue(info.TotalAmount);

            cell = sheet1.GetRow(12).GetCell(1);
            cell.SetCellValue(cell.StringCellValue + info.Sender);
            cell = sheet1.GetRow(12).GetCell(2);
            cell.SetCellValue(cell.StringCellValue + info.Manager);

            FileInfo fi = new FileInfo(downExlPath);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }


            //创建文件
            FileStream files = new FileStream(downExlPath, FileMode.Create);
            workbook.Write(files);
            files.Close();


        }


        //public static string  ExportSaleReport()
        //{

        //}

    }
}
