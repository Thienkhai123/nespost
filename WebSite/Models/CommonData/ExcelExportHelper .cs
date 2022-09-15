using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
namespace WebSite.Models
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
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

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, string Header="", bool showFooter=false, decimal ToTalAmount=0,decimal TotalVAT=0,decimal ToTal=0, string lag="")
        {

            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0}", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("STT", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }


                // add the content into the Excel file 
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content 
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    workSheet.Column(columnIndex).AutoFit();
                    columnIndex++;
                }

                //// format header - bold, yellow on black 
                //using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                //{
                //    // r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                //    r.Style.Font.Bold = true;
                //    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                //    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                //    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                //    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                //    //r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    // r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("white"));
                //}
                
                // removed ignored columns 
                if (!String.IsNullOrEmpty(heading))
                {
                    ExcelRange r = workSheet.Cells[1, 1, 1, dataTable.Columns.Count];
                    r.Value = heading;
                    r.Merge = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Font.Bold = true;
                    r.Style.Font.Size = 20;
                }
                if (!String.IsNullOrEmpty(Header))
                {
                    ExcelRange r = workSheet.Cells[2, 1, 2, dataTable.Columns.Count];
                    r.Value = Header;
                    r.Merge = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Font.Size = 14;
                }

                if (showFooter)
                {
                    ExcelRange r = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 1, 1, startRowFrom + dataTable.Rows.Count + 1, dataTable.Columns.Count-1];
                    if (lag=="ja")
                    {
                        r.Value = "財とサービスの 総 額";
                    }
                    else if (lag == "en") {
                        r.Value = "Total amount of good and services";
                    }

                    else
                    {
                        r.Value = "Cộng tiền hàng hoá, dịch vụ:";
                    }                    
                    r.Merge = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Font.Bold = true;
                    ExcelRange rAmount = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 1, dataTable.Columns.Count, startRowFrom + dataTable.Rows.Count + 1, dataTable.Columns.Count];
                    rAmount.Value = ToTalAmount.ToString("N0");
                    rAmount.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    rAmount.Style.Font.Bold = true;

                    ExcelRange r1 = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 2, 1, startRowFrom + dataTable.Rows.Count + 2, dataTable.Columns.Count-1];
                    if (lag == "ja")
                    {
                        r1.Value = "付 加 価 値 税(VAT)：";
                    }
                    else if (lag == "en") {
                        r1.Value = "VAT:";
                    }
                    else
                    {
                        r1.Value = "Tiền thuế GTGT (VAT):";
                    }
                    
                    r1.Merge = true;
                    r1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r1.Style.Font.Bold = true;
                    ExcelRange rVAT = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 2, dataTable.Columns.Count, startRowFrom + dataTable.Rows.Count + 2, dataTable.Columns.Count];
                    rVAT.Value = TotalVAT.ToString("N0");
                    rVAT.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    rVAT.Style.Font.Bold = true;
                    ExcelRange r2 = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 3, 1, startRowFrom + dataTable.Rows.Count + 3, dataTable.Columns.Count-1];
                    if (lag == "ja")
                    {
                        r2.Value = "合計：";
                    }
                    else if (lag == "en") {
                        r2.Value = "Total:";
                    }
                    else
                    {
                        r2.Value = "Tổng cước phát sinh:";
                    }                    
                    r2.Merge = true;
                    r2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r2.Style.Font.Bold = true;
                    ExcelRange rTotal = workSheet.Cells[startRowFrom + dataTable.Rows.Count + 3, dataTable.Columns.Count, startRowFrom + dataTable.Rows.Count + 3, dataTable.Columns.Count];
                    rTotal.Value = ToTal.ToString("N0");
                    rTotal.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    rTotal.Style.Font.Bold = true;
                }

                // format cells - add borders 
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count+3, dataTable.Columns.Count])
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

                result = package.GetAsByteArray();
            }
            return result;
        }

        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, Dictionary<string, string> dic = null, string header="",bool showFooter= false, decimal TotalAmount=0, decimal TotalVAT = 0, decimal Total = 0, string lag = "")
        {
            return ExportExcel(ListToDataTable<T>(data, dic), Heading, showSlno, header, showFooter, TotalAmount,TotalVAT,Total, lag);
        }
    }
}