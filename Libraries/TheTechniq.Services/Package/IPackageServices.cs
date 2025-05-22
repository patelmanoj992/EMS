using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTecniQ.Services.Package
{
    public partial interface IPackageServices
    {
        Task<DataSet> fillCombo(string cmbName, string type, string whrcond);
        Task<DataSet> FillLotNo(string whrcnd, int itemId);
        Task<DataSet> FillQualitySerachGrid(string whrcnd, int divisionId);
        Task<DataSet> FillSHADE(string whrcnd, int itemId);
        Task<int> generateChallanNo(string whrcond);
    }
}
