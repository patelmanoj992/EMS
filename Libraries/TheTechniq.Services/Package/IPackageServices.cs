using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTecniQ.Core.Domain.Package;

namespace TheTecniQ.Services.Package
{
    public partial interface IPackageServices
    {
        Task<bool> checkPackingSlipNo(string packingSlipNo);
        Task<DataSet> fillCombo(string cmbName, string type, string whrcond);
        Task<DataSet> FillLotNo(string whrcnd, int itemId);
        Task<DataSet> FillQualitySerachGrid(string whrcnd, int divisionId);
        Task<DataSet> FillSHADE(string whrcnd, int itemId);
        Task<int> generateChallanNo(string whrcond);
        Task<DataSet> getBoxDetail(string qualityItemId, string cmbLotNo, string CmbGradeMaster, string cmbShadeNoDtl);
        Task<int> InsertDetail(clsPackingMaster model);
    }
}
