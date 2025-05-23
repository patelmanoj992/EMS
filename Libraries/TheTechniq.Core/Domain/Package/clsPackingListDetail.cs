using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTecniQ.Core.Domain.Package
{
    public class clsPackingListDetail
    {
        public int PackingSlipDtlID { get; set; }
        public int PackingSlipID { get; set; }
        public string BoxNo { get; set; }
        public double NetWeight { get; set; }
        public double Cheese { get; set; }
        public string Grade { get; set; }
        public int ItemID { get; set; }
        public string LotNo { get; set; }
        public int PInwardDtlID { get; set; }
        public bool ISUsed { get; set; }
        public string StockType { get; set; }
        public double GrossWt { get; set; }
        public double TareWt { get; set; }
        public double CartoonWt { get; set; }
        public double PaperTubeWt { get; set; }
        public string SHADENO { get; set; }
        public double TotalBoxNo { get; set; }
        public string BatchNo { get; set; }
        public string DyingJobCardNo { get; set; }
        public string ShadeName { get; set; }
        public string PalletType { get; set; }
    }

}
