using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheTecniQ.Api.Infrastructure.Extensions;
using TheTecniQ.API.Controllers;
using TheTecniQ.API.Infrastructure.Extensions;
using TheTecniQ.API.Models.Common;
using TheTecniQ.Core;
using TheTecniQ.Core.Domain.Attendance;
using TheTecniQ.Core.Domain.Grid;
using TheTecniQ.Core.Domain.Permissions;
using TheTecniQ.Services.Attendance;
using TheTecniQ.Services.Expense;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using Humanizer;
using SkiaSharp;
using TheTecniQ.Services.Common;
using TheTecniQ.Services.Employees;
using System.Collections.Generic;
using TheTecniQ.Api.Models.RequestModel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace TheTecniQ.Api.Controllers.Reports
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class ReportsController(IAttendanceService employeeAttendanceService, IEMS_tblEmployeeExpenseService eMS_TblEmployeeExpenseService,
        IEmployeeService employeeService,
        ICommonService<EMS_tblEmployeeAttendance> tblEmployeeAttendanceService) : BaseController
    {
        private readonly IAttendanceService _iemployeeAttendanceService = employeeAttendanceService;
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IEMS_tblEmployeeExpenseService _ieMS_TblEmployeeExpenseService = eMS_TblEmployeeExpenseService;
        private readonly ICommonService<EMS_tblEmployeeAttendance> _tblEmployeeAttendanceService = tblEmployeeAttendanceService;


        [HttpPost]
        [Route("[action]")]
        [Permission(Page = (PageName.RptMonthwise), Permission = PagePermission.View)]
        public async Task<IActionResult> List(GridRequestModel objGrid)
        {

            var List = await _iemployeeAttendanceService.GetAllAsync_Rpt(objGrid);

            if (List != null && List.Count > 0)
            {
                int index = 1;
                foreach (var item in List)
                {
                    item.SrNo = index++;
                }
            }
            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse { StatusCode = (int)ApiStatusCode.Status200OK, Data = List });
            }
            else
            {
                if (List != null && List.Count > 0)
                {
                    List.Add(new EMS_tblEmployeeAttendance()
                    {
                        EmployeeName = "Total",
                        TotalAmount = List.Sum(x => x.TotalAmount ?? 0),
                        TotalExpenseAmount = List.Sum(x => x.TotalExpenseAmount ?? 0),
                        TotalExtraAmount = List.Sum(x => x.TotalExtraAmount ?? 0),
                        PayableAmount = List.Sum(x => x.PayableAmount ?? 0),
                        VoucherNo = null
                    }); ;

                }
                objGrid.Filename = "MonthwiseRpt";
                IPagedList<EMS_tblEmployeeAttendance> data = new PagedList<EMS_tblEmployeeAttendance>(List, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Monthwise Report", "MonthwiseRpt"));
            }
        }

        [HttpPost("get-monthwise-summary-rpt")]
        //[Route("[action]")]
        [Permission(Page = (PageName.RptMonthwiseSummaryRpt) , Permission = PagePermission.View)]
        public async Task<IActionResult> MonthwiseSummaryRpt(GridRequestModel objGrid)
        {

            var list = await _iemployeeAttendanceService.GetAllDataAsync_Rpt(objGrid);

            List<dynamic> monthWiseSummary = new List<dynamic>();

            if (list != null && list.Count > 0)
            {
                var months = list
                  .Where(x => x.AttendanceDate != null)
                  .Select(x => x.AttendanceDate.ToString("MMM-yy"))
                  .Distinct()
                  .OrderBy(m => DateTime.ParseExact(m, "MMM-yy", null))
                  .ToList();

                var grouped = list
                    .Where(x => x.AttendanceDate != null)
                    .GroupBy(x => x.AttendanceDate.ToString("MMM-yy"))
                    .OrderBy(g => g.Min(x => x.AttendanceDate))
                    .Select((g, index) => new
                    {
                       // SrNo = index + 1,
                        MonthYear = g.Key,
                        TotalAmountSum = g.Sum(x => x.TotalAmount ?? 0)
                    }).ToList<dynamic>();

                monthWiseSummary = grouped;

                // Add overall grand total row
                
                decimal grandTotalOverall = 0;

                foreach (var month in months)
                {
                    var total = list
                        .Where(x => x.AttendanceDate.ToString("MMM-yy") == month)
                        .Sum(x => x.TotalAmount ?? 0);

                    //grandRow[month] = total;
                    grandTotalOverall += total;
                }
                
                monthWiseSummary.Add(new
                {
                   // SrNo = monthWiseSummary.ToList().Count() + 1,
                    MonthYear = "Grand Total",
                    TotalAmountSum = grandTotalOverall
                });
            }

            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    Data = monthWiseSummary
                });
            }
            else
            {
                objGrid.Filename = "MonthwiseSummary";
                IPagedList<dynamic> data = new PagedList<dynamic>(monthWiseSummary, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Monthwise Summary Report", "MonthwiseSummary"));
            }
        }

        [HttpPost("get-divisionwise-summary-rpt")]
        //[Route("[action]")]
        [Permission(Page = (PageName.RptDivisionWiseSummaryRpt), Permission = PagePermission.View)]
        public async Task<IActionResult> DivisionWiseSummaryRpt(GridRequestModel objGrid)
        {

            var list = await _iemployeeAttendanceService.GetAllDataAsync_Rpt(objGrid);

            var monthWiseDivisionSummary = new List<dynamic>();

            if (list != null && list.Count > 0)
            {
                var months = list
                    .Where(x => x.AttendanceDate != null)
                    .Select(x => x.AttendanceDate.ToString("MMM-yy"))
                    .Distinct()
                    .OrderBy(m => DateTime.ParseExact(m, "MMM-yy", null))
                    .ToList();

                // Get distinct Division
                var Division = list
                    .Where(x => !string.IsNullOrEmpty(x.DivisionName))
                    .Select(x => x.DivisionName)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                foreach (var div in Division)
                {
                    var row = new Dictionary<string, object>();
                    row["Division"] = div;
                    decimal grandTotal = 0;

                    foreach (var month in months)
                    {
                        var total = list
                            .Where(x => x.DivisionName == div &&
                                        x.AttendanceDate.ToString("MMM-yy") == month)
                            .Sum(x => x.TotalAmount ?? 0);

                        row[month] = total;
                        grandTotal += total;
                    }

                    row["GrandTotal"] = grandTotal;
                    monthWiseDivisionSummary.Add(row);
                }

                // Add overall grand total row
                var grandRow = new Dictionary<string, object>();
                grandRow["Division"] = "Grand Total";
                decimal grandTotalOverall = 0;

                foreach (var month in months)
                {
                    var total = list
                        .Where(x => x.AttendanceDate.ToString("MMM-yy") == month)
                        .Sum(x => x.TotalAmount ?? 0);

                    grandRow[month] = total;
                    grandTotalOverall += total;
                }

                grandRow["GrandTotal"] = grandTotalOverall;
                monthWiseDivisionSummary.Add(grandRow);
            }

            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    Data = monthWiseDivisionSummary
                });
            }
            else
            {
                objGrid.Filename = "MonthwiseDivisionSummary";
                IPagedList<dynamic> data = new PagedList<dynamic>(monthWiseDivisionSummary, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Division Summary Report", "MonthwiseDivisionSummary"));
            }
        }

        [HttpPost("get-departmentwise-summary-rpt")]
        //[Route("[action]")]
        [Permission(Page = (PageName.RptDepartmentWiseSummaryRpt), Permission = PagePermission.View)]
        public async Task<IActionResult> DepartmentWiseSummaryRpt(GridRequestModel objGrid)
        {

            var list = await _iemployeeAttendanceService.GetAllDataAsync_Rpt(objGrid);

            var monthWiseDepartmentSummary = new List<dynamic>();

            if (list != null && list.Count > 0)
            {
                var months = list
                    .Where(x => x.AttendanceDate != null)
                    .Select(x => x.AttendanceDate.ToString("MMM-yy"))
                    .Distinct()
                    .OrderBy(m => DateTime.ParseExact(m, "MMM-yy", null))
                    .ToList();

                // Get distinct departments
                var departments = list
                    .Where(x => !string.IsNullOrEmpty(x.DepartmentName))
                    .Select(x => x.DepartmentName)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                foreach (var dept in departments)
                {
                    var row = new Dictionary<string, object>();
                    row["Department"] = dept;
                    decimal grandTotal = 0;

                    foreach (var month in months)
                    {
                        var total = list
                            .Where(x => x.DepartmentName == dept &&
                                        x.AttendanceDate.ToString("MMM-yy") == month)
                            .Sum(x => x.TotalAmount ?? 0);

                        row[month] = total;
                        grandTotal += total;
                    }

                    row["GrandTotal"] = grandTotal;
                    monthWiseDepartmentSummary.Add(row);
                }

                // Add overall grand total row
                var grandRow = new Dictionary<string, object>();
                grandRow["Department"] = "Grand Total";
                decimal grandTotalOverall = 0;

                foreach (var month in months)
                {
                    var total = list
                        .Where(x => x.AttendanceDate.ToString("MMM-yy") == month)
                        .Sum(x => x.TotalAmount ?? 0);

                    grandRow[month] = total;
                    grandTotalOverall += total;
                }

                grandRow["GrandTotal"] = grandTotalOverall;
                monthWiseDepartmentSummary.Add(grandRow);
            }

            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    Data = monthWiseDepartmentSummary
                });
            }
            else
            {
                objGrid.Filename = "MonthwiseDepartmentSummary";
                IPagedList<dynamic> data = new PagedList<dynamic>(monthWiseDepartmentSummary, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Monthwise Department Summary Report", "MonthwiseDepartmentSummary"));
            }

        }

        [HttpPost("get-designationwise-summary-rpt")]
        //[Route("[action]")]
        [Permission(Page = (PageName.RptDesignationWiseSummaryRpt), Permission = PagePermission.View)]
        public async Task<IActionResult> DesignationWiseSummaryRpt(GridRequestModel objGrid)
        {

            var list = await _iemployeeAttendanceService.GetAllDataAsync_Rpt(objGrid);

            var designationMonthSummary = new List<dynamic>();

            if (list != null && list.Count > 0)
            {
                // Get months list 
                var months = list
                    .Where(x => x.AttendanceDate != null)
                    .Select(x => x.AttendanceDate.ToString("MMM-yy"))
                    .Distinct()
                    .OrderBy(m => DateTime.ParseExact(m, "MMM-yy", null))
                    .ToList();

                // Get distinct designation names
                var designations = list
                    .Where(x => !string.IsNullOrEmpty(x.DesignationName))
                    .Select(x => x.DesignationName)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                foreach (var desg in designations)
                {
                    var row = new Dictionary<string, object>();
                    row["Designation"] = desg;
                    decimal totalDays = 0;
                    decimal totalAmount = 0;

                    foreach (var month in months)
                    {
                        decimal days = list
                            .Where(x => x.DesignationName == desg &&
                                        x.AttendanceDate.ToString("MMM-yy") == month)
                            .Sum(x => x.AttendDays ?? 0);

                        decimal amt = list
                            .Where(x => x.DesignationName == desg &&
                                        x.AttendanceDate.ToString("MMM-yy") == month)
                            .Sum(x => x.TotalAmount ?? 0);

                        row[$"{month}_Days"] = days;
                        row[$"{month}_Amt"] = amt;

                        totalDays += days;
                        totalAmount += amt;
                    }

                    row["TotalDays"] = totalDays;
                    row["TotalAmount"] = totalAmount;

                    designationMonthSummary.Add(row);
                }

                // Add grand total row
                var grandRow = new Dictionary<string, object>();
                grandRow["Designation"] = "Grand Total";

                decimal grandDays = 0;
                decimal grandAmt = 0;

                foreach (var month in months)
                {
                    decimal monthDays = list
                        .Where(x => x.AttendanceDate.ToString("MMM-yy") == month)
                        .Sum(x => x.AttendDays ?? 0);

                    decimal monthAmt = list
                        .Where(x => x.AttendanceDate.ToString("MMM-yy") == month)
                        .Sum(x => x.TotalAmount ?? 0);

                    grandRow[$"{month}_Days"] = monthDays;
                    grandRow[$"{month}_Amt"] = monthAmt;

                    grandDays += monthDays;
                    grandAmt += monthAmt;
                }

                grandRow["TotalDays"] = grandDays;
                grandRow["TotalAmount"] = grandAmt;

                designationMonthSummary.Add(grandRow);
            }

            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse
                {
                    StatusCode = (int)ApiStatusCode.Status200OK,
                    Data = designationMonthSummary
                });
            }
            else
            {
                objGrid.Filename = "MonthwiseDesignationSummary";
                IPagedList<dynamic> data = new PagedList<dynamic>(designationMonthSummary, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Monthwise Designation Summary Report", "MonthwiseDesignationSummary"));
            }

        }

        [HttpPost]
        [Route("[action]")]
        [Permission(Page = PageName.RptExpenseRpt, Permission = PagePermission.View)]
        public async Task<IActionResult> KharchiRpt(GridRequestModel objGrid)
        {
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = DateTime.Now;
            if (!string.IsNullOrEmpty(objGrid.DateFilter))
            {
                objGrid.Filters.Add(new SearchGrid
                {
                    FieldName = "AttendanceDate",
                    FieldValue = objGrid.DateFilter + "-" + objGrid.DateFilter,
                    OpType = "DateRange",
                    IsTimeZone = false
                });
                var findDateFil = objGrid.Filters.Where(x => x.FieldName == "AttendanceDate").FirstOrDefault();
                if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
                {
                    string[] dateParts = findDateFil.FieldValue.Split('-');
                    firstDate = DateTime.Parse(dateParts[0].Trim());

                    // Ensure firstDate is the 1st of the month
                    firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);

                    // Find the last day of the month
                    secondDate = new DateTime(firstDate.Year, firstDate.Month, DateTime.DaysInMonth(firstDate.Year, firstDate.Month));

                    findDateFil.FieldValue = firstDate.ToString("dd MMM yyyy") + "-" + secondDate.ToString("dd MMM yyyy");
                }
            }
            var List = await _ieMS_TblEmployeeExpenseService.GetAllAsync_Rpt(firstDate, secondDate, objGrid.EnrollNo);
            if (List != null && List.Count > 0)
            {
                int index = 1;
                foreach (var item in List)
                {
                    item.SrNo = index++;
                }
            }
            if (objGrid.ResponseType == EnumResponseType.JSON)
            {
                return Ok(new ApiResponse { StatusCode = (int)ApiStatusCode.Status200OK, Data = List });
            }
            else
            {
                if (List != null && List.Count > 0)
                {
                    
                    List.Add(new EMS_tblEmployeeExpense()
                    {
                        EmployeeName = "Total",
                        Amount = List.Sum(x => x.Amount ?? 0)
                    });
                }
                objGrid.Filename = "KharchiRpt";
                IPagedList<EMS_tblEmployeeExpense> data = new PagedList<EMS_tblEmployeeExpense>(List, 0, 0, 0);
                return Ok(data.ToListResponse(objGrid, "Kharchi Report", "KharchiRpt"));
            }
        }


        [HttpPost]
        [Route("get-voucher")]
        [Permission(Page = PageName.RptExpenseRpt, Permission = PagePermission.View)]
        public async Task<IActionResult> GetVoucher(ReportRequestModel model)
        {
            var pdfStream = await GenerateSalaryVouchers(model.ids);
            return Ok(pdfStream);
        }


        //[NonAction]
        //public async Task<MemoryStream> GenerateSalaryVoucher(int id)
        //{
        //    var data = await _tblEmployeeAttendanceService.GetByIdAsync(id);
        //    var empData = await _employeeService.GetById(data.EmployeeID);
        //    var stream = new MemoryStream();
        //    QuestPDF.Settings.License = LicenseType.Community;

        //    Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.Margin(30);
        //            page.DefaultTextStyle(x => x.FontSize(12));

        //            page.Content()
        //                .Column(col =>
        //                {

        //                    col.Item().Padding(10).AlignCenter().Text("Salary Payment Voucher").Bold().FontSize(14);

        //                    col.Item().PaddingBottom(10).Row(row =>
        //                    {
        //                        row.RelativeItem().Column(col1 =>
        //                        {
        //                            col1.Item().PaddingBottom(5).Text(txt => txt.Span("Account: 2025").Bold());
        //                            col1.Item().PaddingBottom(5).Text(empData.EmployeeName);
        //                            col1.Item().PaddingBottom(5).Text(data.DepartmentName + " - "+ data.DesignationName);
        //                        });

        //                        row.RelativeItem().AlignRight().Column(col2 =>
        //                        {
        //                            col2.Item().PaddingBottom(5).Text(txt => txt.Span("Enroll No: "+ data.EnrollNo).Bold());
        //                            col2.Item().PaddingBottom(5).Text(txt => txt.Span("Dated: "+ DateTime.Now.ToString("dd/MM/yyyy")+"").Bold());
        //                        });
        //                    });

        //                    col.Item().Table(table =>
        //                    {
        //                        table.ColumnsDefinition(columns =>
        //                        {
        //                            columns.RelativeColumn(3);
        //                            columns.RelativeColumn(1);
        //                        });

        //                        table.Header(header =>
        //                        {
        //                            header.Cell().Element(CellStyle).Text("Particulars").Bold().FontSize(12);
        //                            header.Cell().Element(CellStyle).Text("Amount").Bold().FontSize(12);
        //                        });

        //                        table.Cell().Element(CellStyle).Text("Salary Month: "+ data.AttendanceDate.ToString("dd/MM/yyyy")).Bold();
        //                        table.Cell().Element(CellStyle).Text("");

        //                        table.Cell().Element(CellStyle).Text("Total Amount:").Bold();
        //                        table.Cell().Element(CellStyle).Text(data.TotalAmount.ToDynamicString());

        //                        table.Cell().Element(CellStyle).Text("- Kharchi:");
        //                        table.Cell().Element(CellStyle).Text((data.TotalExpenseAmount??0)>0? data.TotalExpenseAmount.ToDynamicString():"");

        //                        table.Cell().Element(CellStyle).Text("Total Extra Amount:");
        //                        table.Cell().Element(CellStyle).Text((data.TotalExtraAmount ?? 0) > 0 ? data.TotalExtraAmount.ToDynamicString() : "");

        //                        table.Cell().Element(CellStyle).Text("Final Pay:").Bold();
        //                        table.Cell().Element(CellStyle).Text(data.PayableAmount.ToDynamicString());

        //                        table.Cell().Element(CellStyle).Text("Remarks").Bold();
        //                        table.Cell().Element(CellStyle).Text(data.Remarks);
        //                    });
        //                    decimal amount = data.PayableAmount ?? 0;
        //                    int rupees = (int)amount;
        //                    int paise = (int)((amount - rupees) * 100);

        //                    string words = rupees.ToWords() + " rupees";
        //                    if (paise > 0)
        //                    {
        //                        words += " and " + paise.ToWords() + " paise";
        //                    }
        //                    col.Item().Text(txt => txt.Span($"Amount (in words): {words}").Bold());

        //                    col.Item().PaddingTop(15).Row(row =>
        //                    {
        //                        row.RelativeItem().AlignLeft().Text("_____________________________");
        //                        row.RelativeItem().AlignRight().Text("_____________________________");
        //                    });

        //                    col.Item().PaddingTop(15).Row(row =>
        //                    {
        //                        row.RelativeItem().AlignLeft().Text("Signature");
        //                        row.RelativeItem().AlignRight().Text("Authorised Signatory (KOMAL INDUSTRIES)");
        //                    });
        //                });
        //        });
        //    }).GeneratePdf(stream);

        //    stream.Position = 0;
        //    return stream;
        //}

        [NonAction]
        public async Task<MemoryStream> GenerateSalaryVouchers(List<int> ids)
        {
            var stream = new MemoryStream();
            QuestPDF.Settings.License = LicenseType.Community;
            var dataList = await _tblEmployeeAttendanceService.GetAllAsync(q=>q.Where(x=> ids.Contains(x.Id)));
            var empDataList = await _employeeService.GetByIds(dataList.Select(x=>x.EmployeeID).ToList());
            // Convert empDataList to a Dictionary for efficient lookup
            var empDict = empDataList.ToDictionary(e => e.EmployeeID, e => e.EnrollNo);

            // Update dataList directly using LINQ's Select (avoiding loop)
            dataList = dataList
                .Select(attendance =>
                {
                    if (empDict.TryGetValue(attendance.EmployeeID, out var enrollNo))
                    {
                        attendance.EnrollNo = enrollNo;
                    }
                    return attendance;
                })
                .ToList();
            Document.Create(container =>
            {
                foreach (var data in dataList.OrderBy(x=>x.EnrollNo)) // Loop for multiple pages
                {
                    var empData = empDataList.Where(x => x.EmployeeID == data.EmployeeID).FirstOrDefault();
                    decimal amount = data.PayableAmount ?? 0;
                    int rupees = (int)amount;
                    int paise = (int)((amount - rupees) * 100);

                    string words = rupees.ToWords() + " rupees";
                    if (paise > 0)
                    {
                        words += " and " + paise.ToWords() + " paise";
                    }
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Content()
                            .Column(col =>
                            {
                                col.Item().Padding(10).AlignCenter().Text("Salary Payment Voucher").Bold().FontSize(14);

                                col.Item().PaddingBottom(5).Row(row =>
                                {
                                    row.RelativeItem().Column(col1 =>
                                    {
                                        col1.Item().PaddingBottom(3).Text(text =>
                                        {
                                            text.Span("Enroll No: ").Bold();
                                            text.Span(empData.EnrollNo.ToString());
                                        }); 
                                        col1.Item().PaddingBottom(3).Text("                    "+empData.EmployeeName).FontSize(10);
                                        col1.Item().PaddingBottom(3).Text("                    " + data.DepartmentName + " - " + data.DesignationName).FontSize(10);
                                    });

                                    row.RelativeItem().PaddingBottom(5).AlignRight().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(120); // Adjust width for the first column
                                            columns.RelativeColumn();    // Allow the second column to take remaining space
                                        });

                                        table.Cell().AlignRight().Text("").Bold();
                                        table.Cell().PaddingRight(48).AlignRight().Text(text =>
                                        {
                                            text.Span("Voucher No: ").Bold();
                                            text.Span(data.Id.ToString());
                                        });

                                        //.Text("Voucher No: "+data.Id.ToString()+ "            ");

                                        table.Cell().AlignRight().Text("").Bold();
                                        table.Cell().AlignRight().Text(text =>
                                        {
                                            text.Span("Dated: ").Bold();
                                            text.Span(DateTime.Now.ToString("dd/MM/yyyy"));
                                        });

                                        //.Text("Dated: " +DateTime.Now.ToString("dd-MM-yyyy"));
                                    });

                                    
                                });

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyleAlignCenter).Text("Particulars").Bold().FontSize(12);
                                        header.Cell().Element(CellStyleAlignCenter).Text("Amount").Bold().FontSize(12);
                                    });

                                    table.Cell().Element(CellStyle).Text("Salary Month:           " + data.AttendanceDate.ToString("MMMM-yyyy")).Bold();
                                    table.Cell().Element(CellStyleAlignCenter).Text(data.PayableAmount.ToDynamicString()).Bold(); 

                                    table.Cell().Element(CellStyleBorderLeftRight).Text("Net Salary:                   " + data.TotalAmount.ToDynamicString()).Bold();
                                    table.Cell().Element(CellStyleBorderLeftRight).Text("");

                                    table.Cell().Element(CellStyleBorderLeftRight).Text("-Kharchi:                        " + ((data.TotalExpenseAmount ?? 0) > 0 ? data.TotalExpenseAmount.ToDynamicString() : ""));
                                    table.Cell().Element(CellStyleBorderLeftRight).Text("");

                                    //table.Cell().Element(CellStyleBorderLeftRight).Text("+Extra Pay:                       " + );
                                    //table.Cell().Element(CellStyleBorderLeftRight).Text("");

                                    table.Cell().Element(CellStyleBorderLeftRight).Text("                                  __________________").Bold();
                                    table.Cell().Element(CellStyleBorderLeftRight).Text("");

                                    table.Cell().Element(CellStyleBorderBottom).Text("Payable Amount:        "+ data.PayableAmount.ToDynamicString());
                                    table.Cell().Element(CellStyleBorderBottom).Text("");

                                    table.Cell().Element(CellStyleRemark).Text(text =>
                                    {
                                        text.Span("Remarks: ").Bold().FontSize(11);
                                        text.Span(data.Remarks + ((data.TotalExtraAmount ?? 0) > 0 ? (" (Extra Pay: " + data.TotalExtraAmount.ToDynamicString() + ")") : "")).FontSize(10);
                                    });
                                    
                                    table.Cell().Element(CellStyleRemark).Text("");

                                    table.Cell().Element(CellStyleBorderNoneAmtWord)
                                    .Text(text =>
                                    {
                                        text.Span("Total Amount (in words): ").Bold().FontSize(10);
                                        text.Span(words).FontSize(10);
                                    });
                                    table.Cell().Element(CellStyleBorderNoneAmtWordAlignCenter).Text(data.PayableAmount.ToDynamicString()).Bold();

                                });

                               
                                //col.Item().Text(txt => txt.Span($"Amount (in words): {words}").Bold());

                                col.Item().PaddingTop(15).Row(row =>
                                {
                                    row.RelativeItem().AlignLeft().Text("_____________________________");
                                    row.RelativeItem().AlignRight().Text("_____________________________");
                                });

                                col.Item().PaddingTop(15).Row(row =>
                                {
                                    row.RelativeItem().AlignLeft().Text("Signature");
                                    row.RelativeItem().AlignRight().Text("Authorised Signatory ");
                                });
                            });
                    });
                }
            }).GeneratePdf(stream);

            stream.Position = 0;
            return stream;
        }



        private static IContainer CellStyle(IContainer container)
        {
            return container.Border(1).Padding(3).AlignLeft();
        }
        private static IContainer CellStyleAlignCenter(IContainer container)
        {
            return container.Border(1).Padding(3).AlignCenter();
        }
        private static IContainer CellStyleBorderLeftRight(IContainer container)
        {
            return container.BorderLeft(1).BorderRight(1).Padding(3).AlignLeft();
        }
        private static IContainer CellStyleBorderBottom(IContainer container)
        {
            return container.BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(3).AlignLeft();
        }

        private static IContainer CellStyleRemark(IContainer container)
        {
            return container.BorderLeft(1).BorderRight(1).BorderBottom(1).PaddingLeft(5).PaddingTop(5).PaddingBottom(10).AlignLeft();
        }
        private static IContainer CellStyleBorderNoneAmtWord(IContainer container)
        {
            return container.Border(1).PaddingLeft(5).PaddingTop(5).PaddingBottom(10).AlignLeft();
        }
        private static IContainer CellStyleBorderNoneAmtWordAlignCenter(IContainer container)
        {
            return container.Border(1).PaddingLeft(5).PaddingTop(5).PaddingBottom(10).AlignCenter();
        }

    }
}
