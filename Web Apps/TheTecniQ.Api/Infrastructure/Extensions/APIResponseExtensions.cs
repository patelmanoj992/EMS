using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

using TheTecniQ.API.Models.Common;
using TheTecniQ.Core;
using System.IO;
using LargeXlsx;
using TheTecniQ.Core.Domain.Logging;
using System.Reflection;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
//using System.Drawing;
using SharpCompress.Compressors.Deflate;
using System.Text;
using DocumentFormat.OpenXml.InkML;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using TheTecniQ.Core.Domain.Grid;
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Runtime.InteropServices;
using TheTecniQ.API.Models.Expense;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using IContainer = QuestPDF.Infrastructure.IContainer;
using Azure.Core;

namespace TheTecniQ.API.Infrastructure.Extensions
{
    public static class APIResponseExtensions
    {
        public static object ToListResponse<T>(this IPagedList<T> list, GridRequestModel objGrid, string msg, string fileName = "")
        {
            return GetListResponse<T>(list, objGrid, msg, fileName);
        }

        public static object ToMobileListResponse<T>(this IPagedList<T> list, MobileGridRequestModel objGrid, string msg, string fileName = "")
        {
            return GetMobileListResponse<T>(list, objGrid, msg, fileName);
        }
        public static ApiResponse GenerateResponse(ApiStatusCode StatusCode, string Message, object Data = null, object AdditionalData = null, string StatusText = "")
        {
            return new ApiResponse() { StatusCode = (int)StatusCode, StatusText = StatusText == "" ? StatusCode.ToString() : StatusText, Message = Message, Data = Data, AdditionalData = AdditionalData };
        }
        public static List<Dictionary<string, object>> ConvertDataTableToList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(row => table.Columns.Cast<DataColumn>()
                            .ToDictionary(col => col.ColumnName, col => row[col]))
                        .ToList();
        }
        public static ApiResponse ToSingleResponse<T, TModel>(this T obj, string msg)
        where T : BaseEntity
        where TModel : BaseModel
        {
            try
            {
                return GenerateResponse(obj is null ? ApiStatusCode.Status404NotFound : ApiStatusCode.Status200OK, obj is null ? "Not Found" : string.Format("Data loaded", msg), obj.MapTo<TModel>());
            }
            catch (Exception)
            {
                return new ApiResponse();
            }
        }
        public static ObjectResult ToMessage(this string msg, ApiStatusCode StatusCode = ApiStatusCode.Status200OK, object additionalData = null)
        {
            var resObj = GetResponse(msg, StatusCode, additionalData);
            return new ObjectResult(resObj)
            {
                StatusCode = resObj.StatusCode
            };
        }

        public static ObjectResult ToResponse<T>(this T obj, string msg, object additionalData = null)
        {
            var resObj = GetResponse<T>(obj, msg, additionalData);
            return new ObjectResult(resObj)
            {
                StatusCode = resObj.StatusCode
            };
        }

        public static ObjectResult ToResponse<T, TModel>(this T obj, string msg, object additionalData = null) where T : BaseEntity where TModel : BaseModel
        {
            var resObj = GetResponse<T, TModel>(obj, msg, additionalData);
            return new ObjectResult(resObj)
            {
                StatusCode = resObj.StatusCode
            };
        }

        public static IList<SelectListItem> ToDropDown<T>(this IList<T> drpList, string dropValColName = "Id", string dropDisColName = "Name", string drpGrpColName = "")
        {
            if (dropValColName.Split('|').Length == 1)
            {
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(), Group = new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" } }).ToList();
            }
            else if (dropValColName.Split('|').Length == 2)
            {
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName.Split('|')[0]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[1]).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(), Group = new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" } }).ToList();
            }
            else if (dropValColName.Split('|').Length == 4)
            {
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName.Split('|')[0]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[1]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[2]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[3]).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(), Group = new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" } }).ToList();
            }
            else
            {
                return drpList.Select(x => new SelectListItem() { Value = (typeof(T).GetProperty(dropValColName.Split('|')[0]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[1]).GetValue(x)).ToString() + "|" + (typeof(T).GetProperty(dropValColName.Split('|')[2]).GetValue(x)).ToString(), Text = typeof(T).GetProperty(dropDisColName).GetValue(x).ToString(), Group = new SelectListGroup() { Name = !string.IsNullOrEmpty(drpGrpColName) ? typeof(T).GetProperty(drpGrpColName).GetValue(x).ToString() : "" } }).ToList();
            }
        }

        #region Utilities

        private static object GetListResponse<T>(this IPagedList<T> list, GridRequestModel objGrid, string msg, string fileName = "")
        {
            if (objGrid.ResponseType == EnumResponseType.Excel)
            {
                
                //objGrid.Columns.Insert(0, new Core.Domain.Grid.GridColumn
                //{
                //    Data = "SrNo",
                //    Name = "SrNo"
                //});
                return ExportToExcel(list, objGrid.Columns, objGrid, "Sheet1");
            }
            else if (objGrid.ResponseType == EnumResponseType.Pdf)
            {
                //objGrid.Columns.Insert(0, new Core.Domain.Grid.GridColumn
                //{
                //    Data = "SrNo",
                //    Name = "SrNo"
                //});
                return ExportToPdf(list, objGrid.Columns, objGrid);
            }
            else if (objGrid.ResponseType == EnumResponseType.Csv)
            {
                return ExportToCsv(list);
            }
            else
            {
                return new
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    StatusText = msg,
                    Data = list,
                    RecordsTotal = list.TotalCount,
                    list.PageIndex
                };
            }
        }

        private static object GetMobileListResponse<T>(this IPagedList<T> list, MobileGridRequestModel objGrid, string msg, string fileName = "")
        {
            if (objGrid.ResponseType == EnumResponseType.Excel)
            {
                return MobileExportToExcel(list, objGrid.Columns, objGrid, "Sheet1");
            }
            //else if (objGrid.ResponseType == EnumResponseType.Pdf)
            //{
            //    //return ExportToPdf(list, objGrid.Columns, objGrid);
            //}
            else if (objGrid.ResponseType == EnumResponseType.Csv)
            {
                return ExportToCsv(list);
            }
            else
            {
                return new
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    StatusText = msg,
                    Data = list,
                    RecordsTotal = list.TotalCount,
                    list.PageIndex
                };
            }
        }

        public static ApiResponse GetResponse(this string msg, ApiStatusCode StatusCode = ApiStatusCode.Status200OK, object data = null, object additionalData = null)
        {
            return new ApiResponse() { StatusCode = (int)StatusCode, StatusText = StatusCode.ToString(), Message = msg, Data = data, AdditionalData = additionalData };
        }

        private static ApiResponse GetResponse<T>(this T obj, string msg, object additionalData = null)
        {
            if (obj != null)
            {
                return new ApiResponse() { StatusCode = (int)ApiStatusCode.Status200OK, StatusText = ApiStatusCode.Status200OK.ToString(), Message = msg, Data = obj, AdditionalData = additionalData };
            }
            else
            {
                return new ApiResponse() { StatusCode = (int)ApiStatusCode.Status404NotFound, StatusText = ApiStatusCode.Status404NotFound.ToString(), Message = "Not Found", Data = obj, AdditionalData = null };
            }
        }

        private static ApiResponse GetResponse<T, TModel>(this T obj, string msg, object additionalData = null) where T : BaseEntity where TModel : BaseModel
        {
            if (obj != null)
            {
                return new ApiResponse() { StatusCode = (int)ApiStatusCode.Status200OK, StatusText = ApiStatusCode.Status200OK.ToString(), Message = msg, Data = obj.MapTo<TModel>(), AdditionalData = additionalData };
            }
            else
            {
                return new ApiResponse() { StatusCode = (int)ApiStatusCode.Status404NotFound, StatusText = ApiStatusCode.Status404NotFound.ToString(), Message = "Not Found", Data = obj, AdditionalData = null };
            }
        }

        private static Stream ExportToExcel<T>(IPagedList<T> list, List<Core.Domain.Grid.GridColumn> columns, GridRequestModel objGrid, string sheetName = "Sheet 1")
        {

            if (objGrid.Filename == "DesignationWiseSummaryRpt")
            {
                var dynamicList = list.Select(item => (dynamic)item).ToList();
                return Export_DesignationWiseSummaryRpt(new XLWorkbook(), dynamicList, objGrid.Months);
            }

            int rowIndex = 1;
            using var excel = new XLWorkbook();
            var workSheet = excel.Worksheets.Add(sheetName);
            rowIndex = AddCustomHeader(workSheet, objGrid, rowIndex);
            // Create and style the header row
            columns.RemoveAll(x => x.Data == "");
            AddHeader(workSheet, columns, rowIndex);
            StyleHeaderRow(workSheet, rowIndex);
            rowIndex++;
            // Start from row 2 because row 1 is the header

            foreach (var item in list)
            {
                PopulateRow(workSheet, item, columns, objGrid, rowIndex);
                workSheet.Row(rowIndex).Height = 15;
                rowIndex++;
            }

            return SaveToStream(excel);
        }

        private static Stream MobileExportToExcel<T>(IPagedList<T> list, List<Core.Domain.Grid.GridColumn> columns, MobileGridRequestModel objGrid, string sheetName = "Sheet 1")
        {
            // Start from row 2 because row 1 is the header
            int rowIndex = 2;
            using var excel = new XLWorkbook();
            var workSheet = excel.Worksheets.Add(sheetName);

            // Create and style the header row
            AddHeader(workSheet, columns, rowIndex);
            StyleHeaderRow(workSheet, rowIndex);


            foreach (var item in list)
            {
                MobilePopulateRow(workSheet, item, columns, objGrid, rowIndex);
                workSheet.Row(rowIndex).Height = 15;
                rowIndex++;
            }

            return SaveToStream(excel);
        }

        private static void AddHeader(IXLWorksheet workSheet, List<Core.Domain.Grid.GridColumn> columns, int rowIndex)
        {
            int colIndex = 1;
            foreach (var column in columns.Where(c => !c.Name.Equals("Action", StringComparison.OrdinalIgnoreCase)))
            {
                workSheet.Cell(rowIndex, colIndex++).Value = column.Name;
            }
        }
        private static int AddCustomHeader(IXLWorksheet workSheet, GridRequestModel objGrid, int startRow = 1)
        {
            if (objGrid.Filename == "MonthwiseRpt")
            {
                var range = workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, objGrid.Columns.Where(c => !c.Name.Equals("Action", StringComparison.OrdinalIgnoreCase)).Count())).Merge();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Font.Bold = true;
                var findDateFil = objGrid.Filters.Where(x => x.FieldName == "AttendanceDate").FirstOrDefault();
                DateTime firstDate = new DateTime();
                if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
                {
                    string[] dateParts = findDateFil.FieldValue.Split('-');
                    firstDate = DateTime.Parse(dateParts[0].Trim());

                    // Ensure firstDate is the 1st of the month
                    firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);
                    
                }
                workSheet.Cell(1, 1).Value = "Salary " + firstDate.ToString("MMM yyyy");
                startRow += 1;
            }
            if (objGrid.Filename == "DivisionWiseRpt" || objGrid.Filename == "DepartmentwiseRpt" || objGrid.Filename == "DesignationwiseRpt")
            {
                var range = workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, objGrid.Columns.Where(c => !c.Name.Equals("Action", StringComparison.OrdinalIgnoreCase)).Count())).Merge();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Font.Bold = true;
                var findDateFil = objGrid.Filters.Where(x => x.FieldName == "AttendanceDate").FirstOrDefault();
                DateTime firstDate = new DateTime();
                if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
                {
                    string[] dateParts = findDateFil.FieldValue.Split('-');
                    firstDate = DateTime.Parse(dateParts[0].Trim());

                    // Ensure firstDate is the 1st of the month
                    firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);
                    
                }
                workSheet.Cell(1, 1).Value = objGrid.PreConcateData+" Summary Report " + firstDate.ToString("MMM yyyy");
                startRow += 1;
            }
            return startRow;
        }

        private static void StyleHeaderRow(IXLWorksheet workSheet, int rowIndex)
        {
            var headerRow = workSheet.Row(rowIndex);
            headerRow.Style.Font.Bold = true;
            //headerRow.Height = 30;
        }

        //private static void PopulateRow<T>(IXLWorksheet workSheet, T item, List<Core.Domain.Grid.GridColumn> columns, GridRequestModel objGrid, int rowIndex)
        //{
        //    var propInfoList = item.GetType().GetProperties();
        //    int colIndex = 1;
        //    Type itemType = item.GetType();
        //    bool isDictionary = typeof(IDictionary<string, object>).IsAssignableFrom(itemType);
        //    if (isDictionary)
        //    {
        //        if (item is IDictionary<string, object> dict)
        //        {
        //            foreach (var column in columns)
        //            {
        //                if (dict.ContainsKey(column.Data))
        //                {
        //                    AddCellValue(workSheet, , column.Data, dict[column.Data], objGrid, rowIndex, colIndex++);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var column in columns)
        //        {
        //            if (!column.Name.Equals("Action", StringComparison.OrdinalIgnoreCase) && propInfoList.Any(x => string.Equals(x.Name, column.Data, StringComparison.OrdinalIgnoreCase)))
        //            {
        //                var propInfo = propInfoList.FirstOrDefault(x => string.Equals(x.Name, column.Data, StringComparison.OrdinalIgnoreCase));
        //                if (propInfo != null)
        //                {
        //                    AddCellValue(workSheet, propInfo, item, objGrid, rowIndex, colIndex++);
        //                }
        //            }
        //        }
        //    }
        //}
        private static void PopulateRow<T>(IXLWorksheet workSheet, T item, List<Core.Domain.Grid.GridColumn> columns, GridRequestModel objGrid, int rowIndex)
        {
            int colIndex = 1;
            Type itemType = item.GetType();
            bool isDictionary = typeof(IDictionary<string, object>).IsAssignableFrom(itemType);

            if (isDictionary && item is IDictionary<string, object> dict)
            {
                foreach (var column in columns)
                {
                    if (column.Name.Equals("Action", StringComparison.OrdinalIgnoreCase)) continue;

                    if (dict.ContainsKey(column.Data))
                    {
                        AddCellValueFromDictionary(workSheet, column.Data, dict[column.Data], objGrid, rowIndex, colIndex++);
                    }
                }
            }
            else
            {
                var propInfoList = item.GetType().GetProperties();
                foreach (var column in columns)
                {
                    if (column.Name.Equals("Action", StringComparison.OrdinalIgnoreCase)) continue;

                    var propInfo = propInfoList.FirstOrDefault(x => string.Equals(x.Name, column.Data, StringComparison.OrdinalIgnoreCase));
                    if (propInfo != null)
                    {
                        AddCellValue(workSheet, propInfo, item, objGrid, rowIndex, colIndex++);
                    }
                }
            }
        }
        private static void MobilePopulateRow<T>(IXLWorksheet workSheet, T item, List<Core.Domain.Grid.GridColumn> columns, MobileGridRequestModel objGrid, int rowIndex)
        {
            var propInfoList = item.GetType().GetProperties();
            int colIndex = 1;

            foreach (var column in columns)
            {
                if (!column.Name.Equals("Action", StringComparison.OrdinalIgnoreCase) && propInfoList.Any(x => string.Equals(x.Name, column.Data, StringComparison.OrdinalIgnoreCase)))
                {
                    var propInfo = propInfoList.FirstOrDefault(x => string.Equals(x.Name, column.Data, StringComparison.OrdinalIgnoreCase));
                    if (propInfo != null)
                    {
                        AddMobileCellValue(workSheet, propInfo, item, objGrid, rowIndex, colIndex++);
                    }
                }
            }
        }

        private static void AddMobileCellValue(IXLWorksheet workSheet, PropertyInfo propInfo, object item, MobileGridRequestModel objGrid, int rowIndex, int colIndex)
        {
            var auditProps = propInfo.GetCustomAttribute<AuditLogAttribute>();
            var value = propInfo.GetValue(item);
            if (value != null)
            {
                if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                {
                    DateTime cellValue = GetMobileDateTimeValue(propInfo, item, auditProps, objGrid);
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = auditProps?.ExportFormat ?? "dd MMM yyyy h:mm tt";
                    if (propInfo.Name == "AttendanceDate" || propInfo.Name == "ExpenseDate")
                    { workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString("MMM-yyyy"); }
                    else
                        workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString(auditProps?.ExportFormat ?? "dd MMM yyyy h:mm tt");
                }
                else if (propInfo.PropertyType == typeof(TimeSpan) || propInfo.PropertyType == typeof(TimeSpan?))
                {
                    TimeSpan cellValue = GetMobileTimeSpanValue(propInfo, item, auditProps, objGrid);
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = auditProps?.ExportFormat ?? "h:mm";
                    workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString(auditProps?.ExportFormat ?? "h:mm");
                }
                else if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
                {
                    workSheet.Cell(rowIndex, colIndex).Value = ((value?.ToString().ToLower() == "true") ? "Yes" : "No");
                }
                else
                {
                    workSheet.Cell(rowIndex, colIndex).Value = value.ToString();
                }

                workSheet.Cell(rowIndex, colIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }

        private static void AddCellValue(IXLWorksheet workSheet, PropertyInfo propInfo, object item, GridRequestModel objGrid, int rowIndex, int colIndex)
        {
            var auditProps = propInfo.GetCustomAttribute<AuditLogAttribute>();
            var value = propInfo.GetValue(item);
            
            if (value != null)
            {
                if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                {
                    DateTime cellValue = GetDateTimeValue(propInfo, item, auditProps, objGrid);
                    if (!((auditProps?.IsIgnoreTimeZone) ?? false))
                        cellValue = cellValue.AddHours(5).AddMinutes(30);
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = auditProps?.ExportFormat ?? "dd MMM yyyy h:mm tt";
                    if (propInfo.Name == "AttendanceDate" || propInfo.Name == "ExpenseDate")
                    { workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString("MMM-yyyy"); }
                    else
                        workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString(auditProps?.ExportFormat ?? "dd MMM yyyy h:mm tt");
                }
                else if (propInfo.PropertyType == typeof(TimeSpan) || propInfo.PropertyType == typeof(TimeSpan?))
                {
                    TimeSpan cellValue = GetTimeSpanValue(propInfo, item, auditProps, objGrid);
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = auditProps?.ExportFormat ?? "h:mm";
                    workSheet.Cell(rowIndex, colIndex).Value = cellValue.ToString(auditProps?.ExportFormat ?? "h:mm");
                }
                else if (value is decimal || value is double || value is float)
                {
                    //return string.Format("{0:0.##}", propValue); // Remove trailing zeros
                    workSheet.Cell(rowIndex, colIndex).Value = string.Format("{0:0.##}", value);
                }
                else if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
                {
                    workSheet.Cell(rowIndex, colIndex).Value = ((value?.ToString().ToLower() == "true") ? "Yes" : "No");
                }
                else
                {
                    workSheet.Cell(rowIndex, colIndex).Value = value.ToString();
                }

                workSheet.Cell(rowIndex, colIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }
        private static void AddCellValueFromDictionary(IXLWorksheet workSheet, string columnName, object value, GridRequestModel objGrid, int rowIndex, int colIndex)
        {
            if (value != null)
            {
                if (value is DateTime dt)
                {
                    dt = dt.AddHours(5).AddMinutes(30); // Apply timezone
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = "dd MMM yyyy h:mm tt";
                    if (columnName == "AttendanceDate" || columnName == "ExpenseDate")
                        workSheet.Cell(rowIndex, colIndex).Value = dt.ToString("MMM-yyyy");
                    else
                        workSheet.Cell(rowIndex, colIndex).Value = dt.ToString("dd MMM yyyy h:mm tt");
                }
                else if (value is TimeSpan ts)
                {
                    workSheet.Cell(rowIndex, colIndex).Style.NumberFormat.Format = "h:mm";
                    workSheet.Cell(rowIndex, colIndex).Value = ts.ToString("h\\:mm");
                }
                else if (value is decimal || value is double || value is float)
                {
                    workSheet.Cell(rowIndex, colIndex).Value = string.Format("{0:0.##}", value);
                }
                else if (value is bool b)
                {
                    workSheet.Cell(rowIndex, colIndex).Value = b ? "Yes" : "No";
                }
                else
                {
                    workSheet.Cell(rowIndex, colIndex).Value = value.ToString();
                }

                workSheet.Cell(rowIndex, colIndex).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }
        private static DateTime GetDateTimeValue(PropertyInfo propInfo, object item, AuditLogAttribute auditProps, GridRequestModel objGrid)
        {
            DateTime dateTimeValue = Convert.ToDateTime(propInfo.GetValue(item));
            return auditProps?.IsIgnoreTimeZone == true ? dateTimeValue : dateTimeValue.ToLocalDateTime(objGrid.Timezone);
        }
        private static DateTime GetMobileDateTimeValue(PropertyInfo propInfo, object item, AuditLogAttribute auditProps, MobileGridRequestModel objGrid)
        {
            DateTime dateTimeValue = Convert.ToDateTime(propInfo.GetValue(item));
            return auditProps?.IsIgnoreTimeZone == true ? dateTimeValue : dateTimeValue.ToLocalDateTime(objGrid.Timezone);
        }

        private static TimeSpan GetTimeSpanValue(PropertyInfo propInfo, object item, AuditLogAttribute auditProps, GridRequestModel objGrid)
        {
            TimeSpan timeSpanValue = Convert.ToDateTime(propInfo.GetValue(item).ToString()).TimeOfDay;
            return auditProps?.IsIgnoreTimeZone == true ? timeSpanValue : Convert.ToDateTime(timeSpanValue).ToLocalDateTime(objGrid.Timezone).TimeOfDay;
        }
        private static TimeSpan GetMobileTimeSpanValue(PropertyInfo propInfo, object item, AuditLogAttribute auditProps, MobileGridRequestModel objGrid)
        {
            TimeSpan timeSpanValue = Convert.ToDateTime(propInfo.GetValue(item).ToString()).TimeOfDay;
            return auditProps?.IsIgnoreTimeZone == true ? timeSpanValue : Convert.ToDateTime(timeSpanValue).ToLocalDateTime(objGrid.Timezone).TimeOfDay;
        }
        public static Stream Export_DesignationWiseSummaryRpt(XLWorkbook excel, List<dynamic> data, List<string> months)
        {
            var worksheet = excel.Worksheets.Add("Summary");
            int row = 1, col = 1;
            

            worksheet.Cell(row, col++).Value = "Designation";

            foreach (var month in months)
            {
                worksheet.Cell(row, col++).Value = $"{month}_Days";
                worksheet.Cell(row, col++).Value = $"{month}_Amt";
            }

            worksheet.Cell(row, col++).Value = "Total Days";
            worksheet.Cell(row, col++).Value = "Total Amount";


            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(data.Count+1).Style.Font.Bold = true;
            foreach (var item in data)
            {
                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = GetPropValue(item, "Designation");

                foreach (var month in months)
                {
                    worksheet.Cell(row, col++).Value = GetPropValue(item, $"{month}_Days");
                    worksheet.Cell(row, col++).Value = GetPropValue(item, $"{month}_Amt");
                }

                worksheet.Cell(row, col++).Value = GetPropValue(item, "TotalDays");
                worksheet.Cell(row, col++).Value = GetPropValue(item, "TotalAmount");
            }

            var stream = new MemoryStream();
            excel.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        private static object GetPropValue(object obj, string propName)
        {
            if (obj is IDictionary<string, object> dict)
                return dict.ContainsKey(propName) ? dict[propName] : null;

            var prop = obj?.GetType().GetProperty(propName);
            return prop?.GetValue(obj, null);
        }

        public static Stream SaveToStream(XLWorkbook excel)
        {
            var memoryStream = new MemoryStream();
            excel.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        //private static System.IO.MemoryStream ExportToPdf<T>(IPagedList<T> list, List<Core.Domain.Grid.GridColumn> Columns)
        //{
        //StringBuilder html = new();
        //html.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
        //   "<head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'><meta name='viewport' content='width=device-width'><meta http-equiv='X-UA-Compatible' content='IE=edge'>" +
        //   "<title>SamplePdf</title></head><body><table cellpadding='5' border='1' style='border-spacing:0px;'>");

        ////add header row
        //html.Append("<tr>");
        //foreach (var Column in Columns)
        //{
        //    html.Append("<th>" + Column.Name + "</th>");
        //}
        //html.Append("</tr>");
        //foreach (T item in list)
        //{
        //    html.Append("<tr>");
        //    System.Reflection.PropertyInfo[] propInfoList = item.GetType().GetProperties();
        //    foreach (var Column in Columns)
        //    {
        //        if (propInfoList.Any(x => x.Name == Column.Data))
        //        {
        //            System.Reflection.PropertyInfo propInfo = propInfoList.FirstOrDefault(x => x.Name == Column.Data);
        //            html.Append("<td>" + propInfo.GetValue(item) + "</td>");
        //        }

        //    }
        //    html.Append("</tr>");
        //}
        //html.Append("</table></body></html>");
        //HtmlConverter htmlConverter = new(html.ToString());
        //byte[] outputByte = htmlConverter.Convert();
        //MemoryStream ms = new(outputByte)
        //{
        //    Position = 0
        //};
        //return ms;
        //}



        private static MemoryStream ExportToPdf<T>(IPagedList<T> list, List<Core.Domain.Grid.GridColumn> Columns, GridRequestModel objGrid)
        {
            Columns.RemoveAll(x => x.Data == "");
            string headerTitle = "";
            if (objGrid.Filename == "MonthwiseRpt")
            {
                var findDateFil = objGrid.Filters.Where(x => x.FieldName == "AttendanceDate").FirstOrDefault();
                DateTime firstDate = new DateTime();
                if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
                {
                    string[] dateParts = findDateFil.FieldValue.Split('-');
                    firstDate = DateTime.Parse(dateParts[0].Trim());

                    // Ensure firstDate is the 1st of the month
                    firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);

                }
                headerTitle = "Salary " + firstDate.ToString("MMM yyyy");                
            }
            else if (objGrid.Filename == "KharchiRpt")
            {
                headerTitle = "Kharchi Report";
            }
            QuestPDF.Settings.License = LicenseType.Community;

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); // Landscape for better width
                    page.Margin(10);

                    // Header
                    page.Header().Element(header =>
                    {
                        header.AlignCenter()
                            .PaddingBottom(5) // Now padding applies properly
                            .Text(headerTitle)
                            .FontSize(10)
                            .Bold();
                    });

                    page.Content().Table(table =>
                    {
                        // Dynamic Column Definition
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var col in Columns)
                            {
                                if (col.Data == "EmployeeName")
                                    columns.RelativeColumn(2.5f); // Give more space to Employee Name
                                else if (col.Data == "AdharcardNo")
                                    columns.RelativeColumn(1.8f);
                                else if (col.Data == "DepartmentName")
                                    columns.RelativeColumn(1.8f);
                                else if (col.Data == "DesignationName")
                                    columns.RelativeColumn(1.8f); // Give extra space for Aadhaar No
                                else
                                    columns.RelativeColumn(1); // Set a fixed width for other columns
                            }
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            foreach (var column in Columns)
                            {
                                header.Cell()
                                    .Element(CellAlignCenterStyle)
                                    .Text(column.Name ?? column.Name)
                                    .FontSize(10)
                                    .Bold();
                            }
                        });

                        foreach (var item in list)
                        {
                            var propInfoList = item.GetType().GetProperties();

                            bool isLastRow = list.IndexOf(item) == list.Count - 1;// Check if this is the last row

                            foreach (var column in Columns)
                            {
                                var propInfo = propInfoList.FirstOrDefault(x => x.Name == column.Data);
                                string value = GetFormattedValue(propInfo, item, objGrid);
                                
                                if (column.Data == "EmployeeName" || column.Data == "DepartmentName" || column.Data == "DesignationName")
                                {
                                    if (isLastRow)
                                        table.Cell()
                                    .Element(isLastRow ? BoldCellStyle : CellAlignCenterStyle) // Apply bold style for the last row
                                    .Text(value).FontSize((isLastRow ? 10:9f));
                                    else
                                        table.Cell()
                                    .Element(CellStyleAlignleft) // Apply bold style for the last row
                                    .Text(value).FontSize((isLastRow ? 10 : 9f));
                                }
                                else {
                                    table.Cell()
                                       .Element(isLastRow ? BoldCellStyle : CellAlignCenterStyle) // Apply bold style for the last row
                                       .Text(value).FontSize((isLastRow ? 10 : 8f));
                                }
                                
                            }
                        }
                    });

                    // Footer
                    page.Footer()
                        .AlignRight()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                });
            }).GeneratePdf();

            return new MemoryStream(pdfBytes);
        }


        

        // Styling method for table cells
        private static IContainer CellAlignCenterStyle(IContainer container) => container
            .Border(0.5f)
            .BorderColor(Colors.Black)
            .Padding(3)
            .AlignCenter();

        private static IContainer CellStyleAlignleft(IContainer container) => container
            .Border(0.5f)
            .BorderColor(Colors.Black)
            .Padding(3)
            .AlignLeft();
        private static IContainer CellStyleAlignRight(IContainer container) => container
            .Border(0.5f)
            .BorderColor(Colors.Black)
            .Padding(3)
            .AlignRight();
        private static IContainer BoldCellStyle(IContainer container) =>
    container
        .Border(1) // Ensure border is applied
        .BorderColor(Colors.Black)
        .Background(Colors.Grey.Lighten2)
        .Padding(3)
        .AlignCenter()
        .DefaultTextStyle(x => x.Bold());

        // Formats values based on their data type
        private static string GetFormattedValue<T>(PropertyInfo propInfo, T item, GridRequestModel objGrid)
        {
            if (propInfo == null) return "";

            object propValue = propInfo.GetValue(item);
            if (propValue == null) return "";

            if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
            {
                DateTime cellValue = (DateTime)propValue;
                if (objGrid.Filename == "KharchiRpt") // Ignore datetime in specific reports
                    return "";

                return cellValue.AddHours(5).AddMinutes(30).ToString("dd MMM yyyy h:mm tt");
            }
            else if (propInfo.PropertyType == typeof(TimeSpan) || propInfo.PropertyType == typeof(TimeSpan?))
            {
                TimeSpan cellValue = (TimeSpan)propValue;
                return cellValue.ToString(@"h\:mm");
            }
            else if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
            {
                return (propValue.ToString().ToLower() == "true") ? "Yes" : "No";
            }
            else if (propValue is decimal || propValue is double || propValue is float)
            {
                return string.Format("{0:0.##}", propValue); // Remove trailing zeros
            }

            return propValue.ToString();
        }


        private static void LoadWkHtmlToX()
        {
            var dllPath = System.IO.Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dll");

            if (File.Exists(dllPath))
            {
                NativeLibrary.Load(dllPath);
                Console.WriteLine("libwkhtmltox.dll loaded successfully!");
            }
            else
            {
                throw new FileNotFoundException("libwkhtmltox.dll not found in the application directory.");
            }
        }

        private static System.IO.MemoryStream ExportToCsv<T>(IPagedList<T> list)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(list);
            }
            return memoryStream;
        }

        #endregion
    }
}