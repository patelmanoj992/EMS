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
using Microsoft.VisualBasic;

namespace TheTecniQ.Services.Package
{
    public partial class PackageServices() : IPackageServices
    {
        #region Fields
        
        #endregion



        #region Methods

        #region Get

        
        public async Task<bool> CheckEnrollNo(string EnrollNo, int? Id)
        {
            MsSqlDataProvider objSql = new();
            var sqlQry = @"select COUNT(0) from tblEmployee WHERE EnrollNo Like '%"+ EnrollNo + "%' AND EmployeeID != "+Id;
            return (await objSql.QueryEntityAsync<int>(sqlQry, null) > 0);
        }


        public async Task<DataSet> FillQualitySerachGrid(string whrcnd,int divisionId)
        {
            whrcnd = (!string.IsNullOrEmpty(whrcnd)?"And ItemName like '%" + whrcnd + "%'":"") + "  and DivisionID=" + divisionId + " Order by ItemName ";
            string query = "Select ItemID, ItemName from tblItems where 1=1 " + whrcnd;
            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> FillLotNo(string whrcnd,int itemId)
        {
            whrcnd = (!string.IsNullOrEmpty(whrcnd) ? " and ItemID= " + itemId : "") + " order by  ItemDtlID desc ,Name ";
            string query = "  select 1000000000000000 as ItemDtLID,'' as Name union select ItemDtlID,NAme  from tblitemdetail where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> FillSHADE(string whrcnd,int itemId)
        {
            string whrcndItem = ((itemId != null && itemId > 0) ? " and ItemID= " + itemId : "");
            whrcnd = (!string.IsNullOrEmpty(whrcnd) ? "And ShadeNo like '%" + whrcnd + "%'" : "");
            whrcnd = whrcndItem + whrcnd + " order by  shadeNo ";
            string query = "Select 0 as  ShadeID,'' as ShadeNo,'' as SHADENAME union  Select ShadeID,ShadeNo,SHADENAME from tblShadeMaster where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> fillCombo(string cmbName, string type, string whrcond)
        {
            //whrcnd = (!string.IsNullOrEmpty(whrcnd) ? " and ItemID= " + itemId : "") + " order by  ItemDtlID desc ,Name ";
            string strquery = "select 0 as GeneralID,'--Select " + cmbName + "--' as Generalvalue union Select GeneralID ,GeneralValue  from tblgeneral where  GeneralType=" + whrcond + " and Remarks like '" + type + "' Order by GeneralValue ";
            //string query = "  select 1000000000000000 as ItemDtLID,'' as Name union select ItemDtlID,NAme  from tblitemdetail where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(strquery, false, null);
        }
        public async Task<int> generateChallanNo(string whrcond)
        {
            string str = "select isnull(max(CONVERT(numeric,PackingSlipAutoNo)),0) + 1 as ChallanNo from tblPackingListMaster where 1=1 " + whrcond;
            MsSqlDataProvider obj = new();
            return await obj.QueryEntityAsync<int>(str,null);
        }
        public async Task<DataSet> getBoxDetail(string cmbLotNo, string CmbGradeMaster, string cmbShadeNoDtl)
        {
            string whrcond = "";
            if (!string.IsNullOrEmpty(cmbLotNo))
                whrcond += " and LotNo='" + cmbLotNo.Trim() + "'";
            if (!string.IsNullOrEmpty(CmbGradeMaster))
                whrcond += " and GRade='" + CmbGradeMaster.Trim() + "'";
            if (!string.IsNullOrEmpty(cmbShadeNoDtl))
                whrcond += "  and ShadeNo='" + cmbShadeNoDtl.Trim() + "' ";
            return new DataSet();
        }


        #endregion

        #endregion
    }
}