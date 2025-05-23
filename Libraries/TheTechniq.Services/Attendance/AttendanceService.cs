using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TheTecniQ.Data;
using TheTecniQ.Core;
using TheTecniQ.Core.Domain.User;
using TheTecniQ.Core.Domain.Grid;
using TheTecniQ.Core.Infrastructure;
using TheTecniQ.Services.Common;
using TheTecniQ.Core.Configuration;
using TheTecniQ.Core.Domain.Notification;
using TheTecniQ.Core.Domain.Employees;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography.Xml;
using System.Data;
using TheTecniQ.Core.Domain.Logging;
using TheTecniQ.Data.DataProviders;
using LinqToDB.Data;
using TheTecniQ.Services.Users;
using TheTecniQ.Core.Domain.Masters;
using EasyNetQ;
using System.Net;
using static LinqToDB.Common.Configuration;
using TheTecniQ.Core.Domain.Attendance;
using Microsoft.IdentityModel.Tokens;

namespace TheTecniQ.Services.Attendance
{
    public partial class AttendanceService(IRepository<EMS_tblEmployeeAttendance> EmployeeAttendanceRepository,
        IRepository<EMS_tblEmployeeExpense> EMS_tblEmployeeExpenseRepository,
        IRepository<EMS_tblAttendanceActivation> EMS_tblAttendanceActivationRepository) : IAttendanceService
    {
        #region Fields
        private readonly IRepository<EMS_tblEmployeeAttendance> _EmployeeAttendanceRepository = EmployeeAttendanceRepository;
        private readonly IRepository<EMS_tblEmployeeExpense> _EMS_tblEmployeeExpenseRepository = EMS_tblEmployeeExpenseRepository;
        private readonly IRepository<EMS_tblAttendanceActivation> _EMS_tblAttendanceActivationRepository = EMS_tblAttendanceActivationRepository;

        #endregion



        #region Methods

        #region Get

        public async Task<IPagedList<EMS_tblEmployeeAttendance>> GetAllAsync(GridRequestModel objGrid)
        {
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = DateTime.Now;

            var findDateFil = objGrid.Filters.FirstOrDefault(x => x.FieldName == "AttendanceDate");
            if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
            {
                string[] dateParts = findDateFil.FieldValue.Split('-');
                firstDate = DateTime.Parse(dateParts[0].Trim());

                // Set firstDate to the 1st of the month
                firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);

                // Get last day of the month
                secondDate = new DateTime(firstDate.Year, firstDate.Month, DateTime.DaysInMonth(firstDate.Year, firstDate.Month));

                findDateFil.FieldValue = $"{firstDate:dd MMM yyyy}-{secondDate:dd MMM yyyy}";
            }

            MsSqlDataProvider objSql = new();
            List<int> employeeId = null;
            var findEnrollFil = objGrid.Filters.Where(x => x.FieldName == "EnrollNo").FirstOrDefault();
            if (!string.IsNullOrEmpty(findEnrollFil?.FieldValue))
            {
                var employee = await objSql.QueryAsync<tblEmployee>(@"select EmployeeID from tblEmployee Where EnrollNo IN (" + findEnrollFil?.FieldValue.Trim() + ") ", null);
                if (employee != null && employee.Count > 0)
                {
                    employeeId = new List<int>();
                    foreach (var item in employee)
                    {
                        employeeId.Add(item.EmployeeID);
                    }
                }
                else
                {
                    employeeId = new List<int>();
                    employeeId.Add(0);
                }
                objGrid.Filters.Remove(findEnrollFil);
            }
            IQueryable<EMS_tblEmployeeAttendance> query = from u in _EmployeeAttendanceRepository.Table
                                                          where (employeeId == null || employeeId.Contains(u.EmployeeID))
                                                          select u;
            objSql = new();
            var data = await _EmployeeAttendanceRepository.GetAllPagedAsync(objGrid, query);
            if (data != null && data.Count > 0)
            {
                var EmployeeIDList = data.Select(x => x.EmployeeID).ToList();
                if (EmployeeIDList != null && EmployeeIDList.Count > 0)
                {
                    var EmployeeIDList1 = data.Select(x => x.EmployeeID).Distinct().ToList();

                    // Fetch total expenses grouped by EmployeeID
                    var totalExpenseList = await _EMS_tblEmployeeExpenseRepository.Table
                        .Where(x => (EmployeeIDList1.Count == 0 || EmployeeIDList1.Contains(x.EmployeeID))
                            && x.ExpenseDate.Date >= firstDate.Date
                            && x.ExpenseDate.Date <= secondDate.Date)
                        .GroupBy(x => x.EmployeeID)
                        .Select(g => new { EmployeeID = g.Key, TotalAmount = g.Sum(x => x.Amount ?? 0) })
                        .ToDictionaryAsync(x => x.EmployeeID, x => x.TotalAmount);

                    // Update rowClass if TotalExpenseAmount mismatch
                    foreach (var item in data.Where(x => totalExpenseList.ContainsKey(x.EmployeeID)))
                    {
                        if (item.TotalExpenseAmount != totalExpenseList[item.EmployeeID])
                        {
                            item.rowClass = "alert-mismatch";
                        }
                    }

                    var dataImageList = await objSql.QueryAsync<tblEmployee>(@"select EmployeeID,EmployeeName,EnrollNo,SalaryType, AdharcardNo from tblEmployee Where EmployeeID IN (" + string.Join(",", EmployeeIDList) + ") ", null);
                    if (dataImageList != null && dataImageList.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            var imgObj = dataImageList.FirstOrDefault(c => c.EmployeeID == item.EmployeeID);
                            if (imgObj != null && imgObj.EmployeeID > 0)
                            {
                                item.EmployeeName = imgObj.EmployeeName;
                                item.EnrollNo = imgObj.EnrollNo;
                                item.SalaryType = imgObj.SalaryType;
                                item.AdharcardNo = imgObj.AdharcardNo;
                            }
                        }
                    }
                }
            }
            return data;

        }

        public async Task<IList<EMS_tblEmployeeAttendance>> GetAllDataAsync_Rpt(GridRequestModel objGrid)
        {

            MsSqlDataProvider objSql = new();
            List<int> employeeId = null;
            var findEnrollFil = objGrid.Filters.Where(x => x.FieldName == "EnrollNo").FirstOrDefault();
            if (!string.IsNullOrEmpty(findEnrollFil?.FieldValue))
            {
                var employee = await objSql.QueryAsync<tblEmployee>(@"select EmployeeID from tblEmployee Where EnrollNo IN (" + findEnrollFil?.FieldValue.Trim() + ") ", null);
                if (employee != null && employee.Count > 0)
                {
                    employeeId = new List<int>();
                    foreach (var item in employee)
                    {
                        employeeId.Add(item.EmployeeID);
                    }
                }
                else
                {
                    employeeId = new List<int>();
                    employeeId.Add(0);
                }
                objGrid.Filters.Remove(findEnrollFil);
            }
            IQueryable<EMS_tblEmployeeAttendance> query = from u in _EmployeeAttendanceRepository.Table
                                                          where (employeeId == null || employeeId.Contains(u.EmployeeID))
                                                          select u;
           // objSql = new();
            var data = await _EmployeeAttendanceRepository.GetAllAsync(query => query);
            List<int> employeeIDs = data? .Where(x => x.EmployeeID != 0).Select(x => x.EmployeeID).Distinct().ToList();
            if (employeeIDs != null && employeeIDs.Count > 0)
            {
                query = query.Where(x => employeeIDs.Contains(x.EmployeeID));
            }

            var result = await query.ToListAsync();
            return result;

        }

        //public async Task<IList<EMS_tblEmployeeAttendance>> GetAllAsync_Rpt(GridRequestModel objGrid)
        //{

        //    DateTime firstDate = DateTime.Now;
        //    DateTime secondDate = DateTime.Now;
        //    var findDateFil = objGrid.Filters.Where(x => x.FieldName == "AttendanceDate").FirstOrDefault();

        //    if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
        //    {
        //        string[] dateParts = findDateFil.FieldValue.Split('-');
        //        firstDate = DateTime.Parse(dateParts[0].Trim());

        //        // Ensure firstDate is the 1st of the month
        //        firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);

        //        // Find the last day of the month
        //        secondDate = new DateTime(firstDate.Year, firstDate.Month, DateTime.DaysInMonth(firstDate.Year, firstDate.Month));

        //        findDateFil.FieldValue = firstDate.ToString("dd MMM yyyy") + "-" + secondDate.ToString("dd MMM yyyy");
        //    }


        //    MsSqlDataProvider objSql = new();
        //    List<int> employeeId = new List<int>();

        //    // Build dynamic WHERE clause
        //    List<string> conditions = new List<string>();

        //    var findEnrollNoFil = objGrid.Filters.FirstOrDefault(x => x.FieldName == "EnrollNo");
        //    if (!string.IsNullOrEmpty(findEnrollNoFil?.FieldValue))
        //    {
        //        conditions.Add($"EnrollNo IN ({findEnrollNoFil.FieldValue.Trim()})");
        //    }

        //    var findDesignationIDFil = objGrid.Filters.FirstOrDefault(x => x.FieldName == "DesignationID");
        //    if (!string.IsNullOrEmpty(findDesignationIDFil?.FieldValue))
        //    {
        //        conditions.Add($"DesignationID IN ({findDesignationIDFil.FieldValue.Trim()})");
        //    }

        //    var findDepartmentIDFil = objGrid.Filters.FirstOrDefault(x => x.FieldName == "DepartmentID");
        //    if (!string.IsNullOrEmpty(findDepartmentIDFil?.FieldValue))
        //    {
        //        conditions.Add($"DepartmentID IN ({findDepartmentIDFil.FieldValue.Trim()})");
        //    }

        //    // Construct final WHERE clause
        //    string whereClause = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";

        //    // Query only if at least one condition exists
        //    if (!string.IsNullOrEmpty(whereClause))
        //    {
        //        var employees = await objSql.QueryAsync<tblEmployee>($"SELECT EmployeeID FROM tblEmployee {whereClause}", null);
        //        if (employees != null && employees.Count > 0)
        //        {
        //            employeeId.AddRange(employees.Select(e => e.EmployeeID));
        //        }
        //        else
        //        {
        //            employeeId.Add(0);
        //        }
        //    }


        //    objSql = new();
        //    var data = await _EmployeeAttendanceRepository.GetAllAsync(query =>
        //        query.Where(x => ((employeeId.Count <= 0) || employeeId.Contains(x.EmployeeID)) && (x.AttendanceDate.Date >= firstDate.Date && x.AttendanceDate.Date <= secondDate.Date)));
        //    if (data != null && data.Count > 0)
        //    {
        //        var EmployeeIDList = data.Select(x => x.EmployeeID).ToList();
        //        if (EmployeeIDList != null && EmployeeIDList.Count > 0)
        //        {

        //            var totalExpenseList = await _EMS_tblEmployeeExpenseRepository.Table
        //                .Where(x => (employeeId.Count <= 0 || employeeId.Contains(x.EmployeeID))
        //                    && x.ExpenseDate.Date >= firstDate.Date
        //                    && x.ExpenseDate.Date <= secondDate.Date)
        //                .GroupBy(x => x.EmployeeID)
        //                .Select(g => new
        //                {
        //                    EmployeeID = g.Key,
        //                    TotalAmount = g.Sum(x => x.Amount ?? 0) 
        //                })
        //                .ToListAsync();

        //            if (totalExpenseList != null && totalExpenseList.Count > 0)
        //            {
        //                var expenseDict = totalExpenseList.ToDictionary(x => x.EmployeeID, x => x.TotalAmount);

        //                // Loop only for employees present in totalExpenseList
        //                foreach (var expense in totalExpenseList)
        //                {
        //                    var item = data.FirstOrDefault(x => x.EmployeeID == expense.EmployeeID);
        //                    if (item != null && item.TotalExpenseAmount != expense.TotalAmount)
        //                    {
        //                        item.rowClass = "alert-mismatch";
        //                    }
        //                }
        //            }

        //            var dataImageList = await objSql.QueryAsync<tblEmployee>(@"select EmployeeID,EmployeeName,EnrollNo,SalaryType, AdharcardNo from tblEmployee Where EmployeeID IN (" + string.Join(",", EmployeeIDList) + ") ", null);
        //            if (dataImageList != null && dataImageList.Count > 0)
        //            {
        //                foreach (var item in data)
        //                {
        //                    var imgObj = dataImageList.FirstOrDefault(c => c.EmployeeID == item.EmployeeID);
        //                    if (imgObj != null && imgObj.EmployeeID > 0)
        //                    {
        //                        item.EmployeeName = imgObj.EmployeeName;
        //                        item.EnrollNo = imgObj.EnrollNo;
        //                        item.SalaryType = imgObj.SalaryType;
        //                        item.AdharcardNo = imgObj.AdharcardNo;
        //                    }
        //                }
        //            }
        //        }
        //        data = data.OrderBy(x => x.EnrollNo).ToList();
        //    }
        //    return data;

        //}

        public async Task<IList<EMS_tblEmployeeAttendance>> GetAllAsync_Rpt(GridRequestModel objGrid)
        {
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = DateTime.Now;

            var findDateFil = objGrid.Filters.FirstOrDefault(x => x.FieldName == "AttendanceDate");
            if (!string.IsNullOrEmpty(findDateFil?.FieldValue))
            {
                string[] dateParts = findDateFil.FieldValue.Split('-');
                firstDate = DateTime.Parse(dateParts[0].Trim());

                // Set firstDate to the 1st of the month
                firstDate = new DateTime(firstDate.Year, firstDate.Month, 1);

                // Get last day of the month
                secondDate = new DateTime(firstDate.Year, firstDate.Month, DateTime.DaysInMonth(firstDate.Year, firstDate.Month));

                findDateFil.FieldValue = $"{firstDate:dd MMM yyyy}-{secondDate:dd MMM yyyy}";
            }

            MsSqlDataProvider objSql = new();
            List<int> employeeIdList = new();

            // Build dynamic WHERE clause
            List<string> conditions = new();
            var filters = new Dictionary<string, string>
    {
        { "EnrollNo", objGrid.Filters.FirstOrDefault(x => x.FieldName == "EnrollNo")?.FieldValue },
        { "DesignationID", objGrid.Filters.FirstOrDefault(x => x.FieldName == "DesignationID")?.FieldValue },
        { "DepartmentID", objGrid.Filters.FirstOrDefault(x => x.FieldName == "DepartmentID")?.FieldValue }
    };

            foreach (var filter in filters)
            {
                if (!string.IsNullOrEmpty(filter.Value))
                {
                    conditions.Add($"{filter.Key} IN ({filter.Value.Trim()})");
                }
            }

            if (conditions.Any())
            {
                string whereClause = " WHERE " + string.Join(" AND ", conditions);
                var employees = await objSql.QueryAsync<tblEmployee>($"SELECT EmployeeID FROM tblEmployee {whereClause}", null);
                employeeIdList = employees?.Select(e => e.EmployeeID).ToList() ?? new List<int> { 0 };
            }

            // Fetch attendance data
            var data = await _EmployeeAttendanceRepository.GetAllAsync(query =>
                query.Where(x => (employeeIdList.Count == 0 || employeeIdList.Contains(x.EmployeeID))
                    && x.AttendanceDate.Date >= firstDate.Date && x.AttendanceDate.Date <= secondDate.Date));

            if (data?.Count > 0)
            {
                var EmployeeIDList = data.Select(x => x.EmployeeID).Distinct().ToList();

                // Fetch total expenses grouped by EmployeeID
                var totalExpenseList = await _EMS_tblEmployeeExpenseRepository.Table
                    .Where(x => (employeeIdList.Count == 0 || employeeIdList.Contains(x.EmployeeID))
                        && x.ExpenseDate.Date >= firstDate.Date
                        && x.ExpenseDate.Date <= secondDate.Date)
                    .GroupBy(x => x.EmployeeID)
                    .Select(g => new { EmployeeID = g.Key, TotalAmount = g.Sum(x => x.Amount ?? 0) })
                    .ToDictionaryAsync(x => x.EmployeeID, x => x.TotalAmount);

                // Update rowClass if TotalExpenseAmount mismatch
                foreach (var item in data.Where(x => totalExpenseList.ContainsKey(x.EmployeeID)))
                {
                    if (item.TotalExpenseAmount != totalExpenseList[item.EmployeeID])
                    {
                        item.rowClass = "alert-mismatch";
                    }
                }

                // Fetch additional employee details
                var dataImageList = await objSql.QueryAsync<tblEmployee>(
                    $"SELECT EmployeeID, EmployeeName, EnrollNo, SalaryType, AdharcardNo FROM tblEmployee WHERE EmployeeID IN ({string.Join(",", EmployeeIDList)})",
                    null);

                var employeeDataDict = dataImageList?.ToDictionary(x => x.EmployeeID);

                // Map employee details
                foreach (var item in data.Where(x => employeeDataDict.ContainsKey(x.EmployeeID)))
                {
                    var empData = employeeDataDict[item.EmployeeID];
                    item.EmployeeName = empData.EmployeeName;
                    item.EnrollNo = empData.EnrollNo;
                    item.SalaryType = empData.SalaryType;
                    item.AdharcardNo = empData.AdharcardNo;
                }

                // Sort by EnrollNo
                data = data.OrderBy(x => x.EnrollNo).ToList();
            }

            return data;
        }


        public async Task<decimal?> GetEmployeeExpense(int employeeId, DateTime dt)
        {
            return await _EMS_tblEmployeeExpenseRepository.Table.Where(x => x.EmployeeID == employeeId && (x.ExpenseDate.Date.Month == dt.Month && x.ExpenseDate.Date.Year == dt.Year)).SumAsync(x => x.Amount);
        }

        public async Task<EMS_tblEmployeeAttendance> GetEmployeeAttendence(int employeeId, DateTime dt)
        {
            return await _EmployeeAttendanceRepository.Table.Where(x => x.EmployeeID == employeeId && (x.AttendanceDate.Date.Month == dt.Month && x.AttendanceDate.Date.Year == dt.Year)).FirstOrDefaultAsync();
        }

        public async Task<IList<DropdDownEmployeeModel>> ActiveEmployeeSearch(SearchActiveEmployeeModel search)
        {
            IList<DropdDownEmployeeModel> data = [];
            string whereClause = " WHERE 1=1 ";
            //if (search.DepartmentID != null && search.DepartmentID > 0)
            //    whereClause += " AND DepartmentID=" + search.DepartmentID;
            //if (search.DivisionID != null && search.DivisionID > 0)
            //    whereClause += " AND DivisionID=" + search.DivisionID;
            //if (search.DesignationID != null && search.DesignationID > 0)
            //    whereClause += " AND DesignationID=" + search.DesignationID;
            //if (!string.IsNullOrEmpty(search.Name))
            //    whereClause += " AND EmployeeName LIKE '%" + search.Name + "%'";
            //if (!string.IsNullOrEmpty(search.EnrollNo))
            //    whereClause += " AND EnrollNo LIKE '%" + search.EnrollNo + "%'";

            var listActiveEmp = await _EMS_tblAttendanceActivationRepository.Table.Where(x => x.AttendanceDate.Date.Month == search.filterDate.Month && x.AttendanceDate.Date.Year == search.filterDate.Year).ToListAsync();
            if (listActiveEmp != null && listActiveEmp.Count > 0)
            {
                var EmployeeIDList = listActiveEmp.Select(x => x.EmployeeID).ToList();
                if (EmployeeIDList != null && EmployeeIDList.Count > 0)
                {
                    MsSqlDataProvider objSql = new();
                    var sqlQry = @"SELECT * FROM View_Employee " + whereClause + " AND EmployeeID IN (" + string.Join(",", EmployeeIDList) + ")  ORDER BY EmployeeName";
                    data = await objSql.QueryAsync<DropdDownEmployeeModel>(sqlQry, null);
                }
            }

            return data;
        }
        #endregion

        #endregion
    }
}