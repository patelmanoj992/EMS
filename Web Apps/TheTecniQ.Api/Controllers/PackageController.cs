using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TheTecniQ.API.Controllers;
using TheTecniQ.API.Infrastructure.Extensions;
using TheTecniQ.API.Models.Common;
using TheTecniQ.Core.Domain.Dyeing;
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
        public async Task<ApiResponse> generateChallanNo(int divisionid, string src = null)
        {
           var dataSet = await _packageServices.generateChallanNo("");
           return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", new { ChallanNo = dataSet });
        }


        [HttpGet("get-qulity/{divisionid}")]
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
        [HttpGet("get-lotno/{itemid}")]
        public async Task<ApiResponse> GetLotNo(int itemid, string src = null)
        {
           var dataSet = await _packageServices.FillLotNo(src, itemid);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-shade/{itemid}")]
        public async Task<ApiResponse> GetShade(int itemid, string src = null)
        {
           var dataSet = await _packageServices.FillSHADE(src, itemid);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
            {
                return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status404NotFound, "No records found.");
            }

            var dataTable = dataSet.Tables[0];
            var dataList = APIResponseExtensions.ConvertDataTableToList(dataTable);
            return APIResponseExtensions.GenerateResponse(ApiStatusCode.Status200OK, "Record found.", dataList);
        }
        [HttpGet("get-grade")]
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
    }
}
