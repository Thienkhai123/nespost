using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace WebSite.Models
{
    public class ExportExcel_ByName
    {
        public static string ExcelContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        public static DataTable ListToDataTable<T>(List<T> data, Dictionary<string, string> dic)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            DataTable dtTBle = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {

                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                dtTBle.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
                dtTBle.Rows.Add(values);
            }


            for (int i = 0; i <= dtTBle.Columns.Count - 1; i++)
            {
                if (!dic.Keys.Contains(dtTBle.Columns[i].ColumnName))
                {
                    dataTable.Columns.Remove(dtTBle.Columns[i].ColumnName);
                }
            }
            for (int i = 0; i <= dataTable.Columns.Count - 1; i++)
            {
                if (dic.Keys.Contains(dataTable.Columns[i].ColumnName))
                {
                    dataTable.Columns[i].ColumnName = dic[dataTable.Columns[i].ColumnName];
                }
            }
            return dataTable;
        }
        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false)
        {

            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0}", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;
                // add the content into the Excel file 

                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content 
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    if (columnIndex < 8 || columnIndex > 16)
                    {

                        workSheet.Cells[startRowFrom - 1, columnIndex, startRowFrom, columnIndex].Merge = true;
                        workSheet.Cells[startRowFrom - 1, columnIndex, startRowFrom, columnIndex].Value = workSheet.GetValue(startRowFrom, columnIndex);

                    }
                    if (columnIndex >= 7 && columnIndex <= 17)
                    {
                        workSheet.Cells[startRowFrom + 1, columnIndex, startRowFrom + dataTable.Rows.Count, columnIndex].Style.Numberformat.Format = "#,##0";
                    }
                    workSheet.Column(columnIndex).AutoFit();
                    columnIndex++;
                }
                workSheet.Cells[startRowFrom - 1, 8, startRowFrom - 1, 16].Merge = true;
                workSheet.Cells[startRowFrom - 1, 8, startRowFrom - 1, 16].Value = "Phụ phí";
                workSheet.Cells[startRowFrom - 1, 8, startRowFrom - 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // format header - bold, yellow on black 
                using (ExcelRange r = workSheet.Cells[startRowFrom - 1, 1, startRowFrom, dataTable.Columns.Count])
                {
                    // r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    //r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    // r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("white"));
                }
                // format cells - add borders 
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns 

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1:Q1"].Value = heading;
                    workSheet.Cells["A1:Q1"].Style.Font.Size = 20;
                    workSheet.Cells["A1:Q1"].Merge = true;
                    workSheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;

                }
                result = package.GetAsByteArray();
            }

            return result;
        }
       
        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, Dictionary<string, string> dic = null)
        {
            return ExportExcel(ListToDataTable<T>(data, dic), Heading, showSlno);
        }
    }
}