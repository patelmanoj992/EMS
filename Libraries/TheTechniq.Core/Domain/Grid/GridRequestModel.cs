using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTecniQ.Core.Domain.Grid
{
    public class GridRequestModel
    {
        public int First { get; set; }
        public int Rows { get; set; }
        public string SortField { get; set; }
        public string DateFilter { get; set; }
        public string EnrollNo { get; set; }
        public string Filename { get; set; }
        public string PreConcateData { get; set; }
        public int SortOrder { get; set; }
        public List<SearchGrid> Filters { get; set; }
        public EnumResponseType ResponseType { get; set; }
        public List<GridColumn> Columns { get; set; }
        public List<string> Months { get; set; }
        public string Timezone { get; set; }
        public GridRequestModel()
        {
            Filters = [];
            First = 0;
            Rows = 10;
        }
    }
    public class GridColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
    }
    public class SearchGrid
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string OpType { get; set; }
        public bool IsTimeZone { get; set; }
    }
    public enum EnumResponseType
    {
        JSON = 0,
        Excel = 1,
        Pdf = 2,
        Word = 3,
        Csv=4
    }

    public class MobileGridRequestModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string Filename { get; set; }
        public int SortOrder { get; set; }
        public List<SearchGrid> Filters { get; set; }
        public EnumResponseType ResponseType { get; set; }
        public List<GridColumn> Columns { get; set; }
        public string Timezone { get; set; }
        public MobileGridRequestModel()
        {
            Filters = [];
            PageNumber = 1;
            PageSize = 30;
        }
    }

}
