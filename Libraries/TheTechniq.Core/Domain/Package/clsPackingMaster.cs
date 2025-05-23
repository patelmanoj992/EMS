using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTecniQ.Core.Domain.Package
{
    public class clsPackingMaster
    {
        public string PackingSlipNo { get; set; }
        public double PackingSlipAutoNo { get; set; }
        public DateTime? PackingSlipDate { get; set; }
        public int ItemID { get; set; }
        public string LotNo { get; set; }
        public string Grade { get; set; }
        public double TotalNetweight { get; set; }
        public double TotalBox { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double TotalCheese { get; set; }
        public string Remarks { get; set; }
        public string SHADENO { get; set; }
        public int DivisionID { get; set; }
        public string MarkaNo { get; set; }
        public List<clsPackingListDetail> clsPackingListDetails { get; set; } = new List<clsPackingListDetail>();   
    }

}
