﻿using NPOI.HSSF.UserModel;
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



        public static void ExportInvoice(string modelExlPath,string downExlPath)
        {
            XSSFWorkbook workbook;
            //读入刚复制的要导出的excel文件
            using (FileStream file = new FileStream(modelExlPath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
                file.Close();
            }
            ISheet sheet1 = workbook.GetSheetAt(0);
            //取模板excel中第二行第二列的单元格样式
            ICellStyle cellstyle = workbook.CreateCellStyle();
            //开始向excel表格中写入数据
            ICell cell = sheet1.GetRow(1).GetCell(1);
            cell.CellStyle = cellstyle;
           // cell.CellStyle.Alignment = HorizontalAlignment.Center;
            IRichTextString rts = new XSSFRichTextString(cell.StringCellValue + "TestWriting");
            IFont  font = workbook.CreateFont();
          //  font.FontHeightInPoints = 20;
            font.Boldweight = 700;
            rts.ApplyFont(0, 3, font);
            cell.SetCellValue(rts);
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
