using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TheTecniQ.Api.Infrastructure.Extensions;
using TheTecniQ.API.Controllers;
using TheTecniQ.API.Infrastructure.Extensions;
using TheTecniQ.API.Models.Common;
using TheTecniQ.Core.Domain.Dyeing;
using TheTecniQ.Core.Domain.Package;
using TheTecniQ.Services.Attendance;
using TheTecniQ.Services.Package;

namespace TheTecniQ.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class PackageController(IPackageServices packageServices) : BaseController
    {
        private readonly IPackageServices _packageServices = packageServices;

        [HttpGet("generate-challanno")]
        [Permission]
        public async Task<ApiResponse> generateChallanNo(int divisionid, string src = null)
        {
            int dataSet = await _packageServices.generateChallanNo("");
            string rtnchallanNO = "MT" + dataSet.ToString("D4");
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", new { ChallanNo = rtnchallanNO });
        }

        [HttpGet("get-qulity/{divisionid}")]
        [Permission]
        public async Task<ApiResponse> GetQuality(int divisionid, string src = null)
        {
            var dataSet = await _packageServices.FillQualitySerachGrid(src, divisionid);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-lotno")]
        [Permission]
        public async Task<ApiResponse> GetLotNo(int? itemid, string src = null)
        {
            var dataSet = await _packageServices.FillLotNo(src, itemid ?? 0);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-shade")]
        [Permission]
        public async Task<ApiResponse> GetShade(int? itemid, string src = null)
        {
            var dataSet = await _packageServices.FillSHADE(src, itemid ?? 0);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-grade")]
        [Permission]
        public async Task<ApiResponse> GetFillCombo()
        {
            var dataSet = await _packageServices.fillCombo("Grade", "Yarn", "'Grade'");
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-boxdetail")]
        //[Permission]
        public async Task<ApiResponse> GetBoxDetail(string q, string l, string g, string s)
        {
            var dataSet = await _packageServices.getBoxDetail(q, l, g, s);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpPost]
        [Permission]
        public async Task<ApiResponse> Post(clsPackingMaster model)
        {
            if (model == null)
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status400BadRequest, "Invalid request data.");

            if (model.clsPackingListDetails == null || model.clsPackingListDetails.Count == 0)
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status400BadRequest, "Please enter at least one boxno detail.");

            if (model.ItemID == null || model.ItemID <= 0)
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status400BadRequest, "Please select an item.");

            if (string.IsNullOrEmpty(model.PackingSlipNo))
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status400BadRequest, "Packing slip number is required.");

            // Check if PackingSlipNo already exists
            if (await _packageServices.checkPackingSlipNo(model.PackingSlipNo))
            {
                int challanNo = await _packageServices.generateChallanNo("");
                model.PackingSlipNo = "MT" + challanNo.ToString("D4");
            }

            model.CreatedBy = CurrentUserId;
            model.CreatedDate = DateTime.Now;

            var result = await _packageServices.InsertDetail(model);

            return result > 0
                ? APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record saved successfully.")
                : APIResponseExtensions.GenerateResponse(ApiStatusCode.Status400BadRequest, "Failed to save record.");

        }

    }
}
