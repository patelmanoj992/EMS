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
using static LinqToDB.DataProvider.MySql.MySqlHints;
using TheTecniQ.Core.Domain.Package;
using System.Security.Cryptography;

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
            var sqlQry = @"select COUNT(0) from tblEmployee WHERE EnrollNo Like '%" + EnrollNo + "%' AND EmployeeID != " + Id;
            return (await objSql.QueryEntityAsync<int>(sqlQry, null) > 0);
        }


        public async Task<DataSet> FillQualitySerachGrid(string whrcnd, int divisionId)
        {
            whrcnd = (!string.IsNullOrEmpty(whrcnd) ? "And ItemName like '%" + whrcnd + "%'" : "") + "  and DivisionID=" + divisionId + " Order by ItemName ";
            string query = "Select ItemID, ItemName from tblItems where 1=1 " + whrcnd;
            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> FillLotNo(string whrcnd, int itemId)
        {
            string whrcndItem = ((itemId != null && itemId > 0) ? " and ItemID= " + itemId : "");
            whrcnd = (!string.IsNullOrEmpty(whrcnd) ? "And NAme like '%" + whrcnd + "%'" : "");
            whrcnd = whrcndItem + whrcnd + " order by  ItemDtlID desc ,Name ";
            string query = "  select ItemDtlID,NAme  from tblitemdetail where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> FillSHADE(string whrcnd, int itemId)
        {
            string whrcndItem = ((itemId != null && itemId > 0) ? " and ItemID= " + itemId : "");
            whrcnd = (!string.IsNullOrEmpty(whrcnd) ? "And ShadeNo like '%" + whrcnd + "%'" : "");
            whrcnd = whrcndItem + whrcnd + " order by  shadeNo ";
            string query = " Select ShadeID,ShadeNo,SHADENAME from tblShadeMaster where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(query, false, null);
        }
        public async Task<DataSet> fillCombo(string cmbName, string type, string whrcond)
        {
            //whrcnd = (!string.IsNullOrEmpty(whrcnd) ? " and ItemID= " + itemId : "") + " order by  ItemDtlID desc ,Name ";
            string strquery = " Select GeneralID ,GeneralValue  from tblgeneral where  GeneralType=" + whrcond + " and Remarks like '" + type + "' Order by GeneralValue ";
            //string query = "  select 1000000000000000 as ItemDtLID,'' as Name union select ItemDtlID,NAme  from tblitemdetail where 1=1 " + whrcnd;

            MsSqlDataProvider obj = new();
            return await obj.ExecuteStoredProcedureForDataSetAsync22(strquery, false, null);
        }
        public async Task<int> generateChallanNo(string whrcond)
        {
            string str = "select isnull(max(CONVERT(numeric,PackingSlipAutoNo)),0) + 1 as ChallanNo from tblPackingListMaster where 1=1 AND ISNULL(IsMobileEntry,0) = 1 " + whrcond;
            MsSqlDataProvider obj = new();
            return await obj.QueryEntityAsync<int>(str, null);
        }
        public async Task<bool> checkPackingSlipNo(string packingSlipNo)
        {
            string str = "Select count(*) from tblPackingListMaster where 1=1 and packingSlipNo='"+ packingSlipNo + "' and PackingSlipID NOT IN (0)";
            MsSqlDataProvider obj = new();
            return (await obj.QueryEntityAsync<int>(str, null) > 0);
        }
        public async Task<DataSet> getBoxDetail(string qualityItemId, string cmbLotNo, string CmbGradeMaster, string cmbShadeNoDtl)
        {
            string whrcond = "";
            if (!string.IsNullOrEmpty(qualityItemId))
                whrcond += " and ItemID=" + qualityItemId.Trim();
            if (!string.IsNullOrEmpty(cmbLotNo))
                whrcond += " and LotNo='" + cmbLotNo.Trim() + "'";
            if (!string.IsNullOrEmpty(CmbGradeMaster))
                whrcond += " and GRade='" + CmbGradeMaster.Trim() + "'";
            if (!string.IsNullOrEmpty(cmbShadeNoDtl))
                whrcond += "  and ShadeNo='" + cmbShadeNoDtl.Trim() + "' ";

            string _InwardDtlIDUsed = "0";
            _InwardDtlIDUsed = " and PInwardDtlID not in (" + _InwardDtlIDUsed + ")";
            string _strNew = "  and PInwardDtlID not in (select PInwardDtlID from tblpackinglistdetail where isnull(ISUsed,0)=0 ) ";
            string finalFilter = _InwardDtlIDUsed + _strNew + " and isnull(isused,0)=0  And isnull(netweight,0)>0" + whrcond;
            MsSqlDataProvider obj = new();
            string str = @" select * from  ( select PalletType,MachineID,inwardtype as StockType,ItemID ,PInwardDtlID ,
                                PInwardID,BoxNo,ItemName,
                            LotNo,Grade,Cheese,NetWeight ,IsUSed,isnull(IsVerify,0) as IsVerify,Isnull(PalletNo,'') as PalletNo,
                            GodownName as Godown,LocationName as Location,
                            Isnull(GodownID,0) as GodownID,Isnull(LocationID,0) as LocationID,
                            SHADENO,Isnull(Isstocktransfer,0) as Isstocktransfer,
                            isnull(ISTRANSFER,0) as ISTRANSFER,TotalBoxNodtl as totalBoxNo,
                            PaperTubeWeight,BoxSizeID,CartoonWeight, GrossWeight,TareWeight,PackingID,
                            isnull(ShadeName,'') as ShadeName,BatchNo,JobCardNo,LotNoNew,Isnull(TermsNo,0) as TermsNo
                             from View_ProductionDetailInward where 1=1  and isnull(isused,0)=0 " + finalFilter + @" ) as a
                              where 1=1 ";
            return await obj.ExecuteStoredProcedureForDataSetAsync22(str, false, null);
        }

        public async Task<int> InsertDetail(clsPackingMaster model)
        {
            MsSqlDataProvider objSql = new();

            DataParameter[] parameters = new List<DataParameter>
            {
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@markaNo", Value = model.MarkaNo },
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@PackingSlipNo", Value = model.PackingSlipNo },
                new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@PackingSlipAutoNo", Value = model.PackingSlipAutoNo },
                new DataParameter { DataType = LinqToDB.DataType.DateTime, Name = "@PackingSlipDate", Value = model.PackingSlipDate },
                new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@ItemID", Value = model.ItemID },
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@LotNo", Value = model.LotNo },
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@Grade", Value = model.Grade },
                new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@TotalNetweight", Value = model.TotalNetweight },
                new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@TotalBox", Value = model.TotalBox },
                new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@CreatedBy", Value = model.CreatedBy },
                new DataParameter { DataType = LinqToDB.DataType.DateTime, Name = "@CreatedDate", Value = model.CreatedDate },
                new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@TotalCheese", Value = model.TotalCheese },
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@Remarks", Value = model.Remarks },
                new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@SHADENO", Value = model.SHADENO },
                new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@DivisionID", Value = model.DivisionID },
                new DataParameter { DataType = LinqToDB.DataType.Boolean, Name = "@IsMobileEntry", Value = true }
            }.ToArray();

            var insertedId = await objSql.ExecuteStoredProcedureForInsertedIdAsync("SP_tblPackingListMasterInsertCustom", CommandType.StoredProcedure, false, "Inserted", parameters);
            if (insertedId != null && insertedId > 0)
            {
                if (model.clsPackingListDetails != null && model.clsPackingListDetails?.Count > 0)
                {
                    foreach (clsPackingListDetail objDtl in model.clsPackingListDetails)
                    {
                        DataParameter[] parametersDetail = new List<DataParameter>
                        {
                            new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@PackingSlipID", Value = insertedId },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@BoxNo", Value = objDtl.BoxNo },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@PalletType", Value = objDtl.PalletType },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@NetWeight", Value = objDtl.NetWeight },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@Cheese", Value = objDtl.Cheese },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@Grade", Value = objDtl.Grade },
                            new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@ItemID", Value = objDtl.ItemID },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@LotNo", Value = objDtl.LotNo },
                            new DataParameter { DataType = LinqToDB.DataType.Int32, Name = "@PInwardDtlID", Value = objDtl.PInwardDtlID },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@StockType", Value = objDtl.StockType },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@GrossWt", Value = objDtl.GrossWt },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@TareWt", Value = objDtl.TareWt },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@CartoonWt", Value = objDtl.CartoonWt },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@PaperTubeWt", Value = objDtl.PaperTubeWt },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@SHADENO", Value = objDtl.SHADENO },
                            new DataParameter { DataType = LinqToDB.DataType.Double, Name = "@TotalBoxNo", Value = objDtl.TotalBoxNo },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@BatchNo", Value = objDtl.BatchNo },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@DyingJobCardNo", Value = objDtl.DyingJobCardNo },
                            new DataParameter { DataType = LinqToDB.DataType.VarChar, Name = "@ShadeName", Value = objDtl.ShadeName }
                        }.ToArray();

                       await objSql.ExecuteStoredProcedureForInsertedIdAsync("SP_tblPackingListDetailInsertCustom", CommandType.StoredProcedure, false, "Inserted", parameters);

                    }
                }
            }

            return insertedId;
        }

        #endregion

        #endregion
    }
}