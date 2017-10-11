using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
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

            MyInsertRow(sheet1, 10, info.Details.Count - 7);

            int index = 3;
            IRow contentRow;
            // ICell contentCell;
            var style = sheet1.GetRow(3).GetCell(3).CellStyle;
            var contentHeight = sheet1.GetRow(3).Height;
            var hCenterStyle = CreateCenterStyle(workbook);
            foreach (var item in info.Details)
            {
                if (index < 10)
                {
                    contentRow = sheet1.GetRow(index);
                }
                else
                {
                    contentRow = sheet1.CreateRow(index);
                    contentRow.Height = contentHeight;
                    for (int i = 0; i < 9; i++)
                    {
                        if (i < 8)
                        {
                            contentRow.CreateCell(i).CellStyle = style;
                        }
                    }
                    sheet1.AddMergedRegion(new CellRangeAddress(index, index, 1, 2));
                    contentRow.GetCell(1).CellStyle = hCenterStyle;
                }
                cell = contentRow.GetCell(1);
                cell.SetCellValue(item.Model);
                cell = contentRow.GetCell(3);
                cell.SetCellValue(item.Quantity);
                cell = contentRow.GetCell(4);
                cell.SetCellValue(item.Unit);
                cell = contentRow.GetCell(5);
                cell.SetCellValue(item.Price);
                cell = contentRow.GetCell(6);
                cell.SetCellValue(item.TotalPrice);
                cell = contentRow.GetCell(7);
                cell.SetCellValue(item.Remark);
                index++;
            }
            
            int bottomIndex = index > 9 ? index : 10;
            cell = sheet1.GetRow(bottomIndex).GetCell(3);
            cell.SetCellValue(info.TotalAmountUp);
            cell = sheet1.GetRow(bottomIndex).GetCell(7);
            cell.SetCellValue(info.TotalAmount);

            cell = sheet1.GetRow(bottomIndex + 2).GetCell(1);
            cell.SetCellValue(cell.StringCellValue + info.Sender);
            cell = sheet1.GetRow(bottomIndex + 2).GetCell(2);
            cell.SetCellValue(cell.StringCellValue + info.Manager);
            sheet1.AddMergedRegion(new CellRangeAddress(2, bottomIndex + 1, 8, 8));
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

        private static ICellStyle CreateCenterStyle(XSSFWorkbook book)
        {
            var style = book.CreateCellStyle();
            style.BorderBottom = style.BorderLeft = style.BorderTop = style.BorderRight = BorderStyle.Thin;
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            return style;
        }

        private static void MyInsertRow(ISheet sheet, int insertRow, int insertRowCount)
        {
            if (insertRowCount <= 0)
            {
                return;
            }
            #region 批量移动行
            sheet.ShiftRows(insertRow, sheet.LastRowNum, insertRowCount, true, false);
            #endregion

            //#region 对批量移动后空出的空行插，创建相应的行，并以插入行的上一行为格式源(即：插入行-1的那一行)
            //for (int i = 插入行; i < 插入行 + 插入行总数 - 1; i++)
            //{
            //    HSSFRow targetRow = null;
            //    HSSFCell sourceCell = null;
            //    HSSFCell targetCell = null;

            //    targetRow = sheet.CreateRow(i + 1);

            //    for (int m = 源格式行.FirstCellNum; m < 源格式行.LastCellNum; m++)
            //    {
            //        sourceCell = 源格式行.GetCell(m);
            //        if (sourceCell == null)
            //            continue;
            //        targetCell = targetRow.CreateCell(m);

            //        targetCell.Encoding = sourceCell.Encoding;
            //        targetCell.CellStyle = sourceCell.CellStyle;
            //        targetCell.SetCellType(sourceCell.CellType);

            //    }
            //    //CopyRow(sourceRow, targetRow);

            //    //Util.CopyRow(sheet, sourceRow, targetRow);
            //}

            //HSSFRow firstTargetRow = sheet.GetRow(插入行);
            //HSSFCell firstSourceCell = null;
            //HSSFCell firstTargetCell = null;

            //for (int m = 源格式行.FirstCellNum; m < 源格式行.LastCellNum; m++)
            //{
            //    firstSourceCell = 源格式行.GetCell(m);
            //    if (firstSourceCell == null)
            //        continue;
            //    firstTargetCell = firstTargetRow.CreateCell(m);

            //    firstTargetCell.Encoding = firstSourceCell.Encoding;
            //    firstTargetCell.CellStyle = firstSourceCell.CellStyle;
            //    firstTargetCell.SetCellType(firstSourceCell.CellType);
            //}
            //#endregion
        }


        //public static string  ExportSaleReport()
        //{

        //}

    }
}
