using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Aspose.Cells;
using BorderType = Aspose.Cells.BorderType;
namespace Game.Common
{
    public class AsposeExcel
    {
        public static void ExportData(string[] head, string[] columnwidth, DataSet ds, string saveFileName)
        {
            string targetFile = string.Format("{0}.xls", saveFileName);
            string targetRoot = string.Format("UploadFile\\ExportExcel");
            var datatable = ds.Tables[0];
            targetRoot = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetRoot);
            if (false == Directory.Exists(targetRoot))
            {
                Directory.CreateDirectory(targetRoot);
            }
            targetFile = System.IO.Path.Combine(targetRoot, targetFile);

            OutFileToDisk(head, columnwidth, datatable, saveFileName, targetFile);

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.BufferOutput = true;
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            System.Web.HttpContext.Current.Response.WriteFile(targetFile);
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;FileName=" + System.Web.HttpUtility.UrlEncode(string.Format("{0}.xls", string.IsNullOrEmpty(saveFileName) ? DateTime.Now.ToString("yyyyMMddHHmmss") : saveFileName), Encoding.UTF8));
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public static void ExportData<T>(string[] columnwidth, Dictionary<string, string> dics, List<T> list, string saveFileName) where T : class
        {
            string targetFile = string.Format("{0}.xls", saveFileName);
            string targetRoot = string.Format("UploadFile\\ExportExcel");

            targetRoot = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetRoot);
            if (false == Directory.Exists(targetRoot))
            {
                Directory.CreateDirectory(targetRoot);
            }
            targetFile = System.IO.Path.Combine(targetRoot, targetFile);

            OutFileToDisk(columnwidth, dics, list, saveFileName, targetFile);

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.BufferOutput = true;
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            System.Web.HttpContext.Current.Response.WriteFile(targetFile);
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;FileName=" + System.Web.HttpUtility.UrlEncode(string.Format("{0}.xls", string.IsNullOrEmpty(saveFileName) ? DateTime.Now.ToString("yyyyMMddHHmmss") : saveFileName), Encoding.UTF8));
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary> 
        /// 导出数据到本地 
        /// </summary> 
        /// <param name="dt">要导出的数据</param> 
        /// <param name="tableName">表格标题</param> 
        /// <param name="path">保存路径</param> 
        public static void OutFileToDisk(string[] head, string[] cloumnwidth, DataTable dt, string tableName, string path)
        {


            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 

            //为标题设置样式     
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //样式2 
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 14;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3 
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数 
            int Rownum = dt.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(tableName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //设置列宽
            for (int j = 0; j < Colnum; j++)
            {
                cells.SetColumnWidth(j, Convert.ToInt32(cloumnwidth[j] ?? "0"));
            }


            //生成行2 列名行 
            for (int i = 0; i < head.Count(); i++)
            {
                //cells[1, i].PutValue(dt.Columns[i].ColumnName);
                cells[1, i].PutValue(head[i]);
                cells[1, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(path);
        }

        public static void OutFileToDisk<T>(string[] columnwidth, Dictionary<string, string> dics, List<T> list, string saveTileName, string path) where T : class
        {
            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 

            //为标题设置样式     
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //样式2 
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 14;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3 
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dics.Count;//表格列数 
            int Rownum = list.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(saveTileName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //设置列宽
            for (int j = 0; j < Colnum; j++)
            {
                cells.SetColumnWidth(j, Convert.ToInt32(columnwidth[j] ?? "0"));
            }


            //生成行2 列名行 
            //for (int i = 0; i < dics.Count(); i++)
            //{
            //    //cells[1, i].PutValue(dt.Columns[i].ColumnName);

            //    cells[1, i].PutValue();
            //    cells[1, i].SetStyle(style2);
            //    cells.SetRowHeight(1, 25);
            //}
            int keyindex = 0;
            foreach (var pro in dics.Keys)
            {

                cells[1, keyindex].PutValue(dics[pro]);
                cells[1, keyindex].SetStyle(style2);
                cells.SetRowHeight(1, 25);
                keyindex++;

            }

            //生成数据行 


            for (int i = 0; i < list.Count; i++)
            {
                int rowkeyindex = 0;
                foreach (var k in dics.Keys)
                {
                    var model = list[i];
                    var value = model.GetType().GetProperty(k) == null ? "" : model.GetType().GetProperty(k).GetValue(model, null) == null ? "" : model.GetType().GetProperty(k).GetValue(model, null).ToString();
                    cells[2 + i, rowkeyindex].PutValue(value);
                    cells[2 + i, rowkeyindex].SetStyle(style3);
                    rowkeyindex++;
                }
                cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(path);
        }
    }
}