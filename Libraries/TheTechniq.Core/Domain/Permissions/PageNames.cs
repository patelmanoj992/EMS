namespace TheTecniQ.Core.Domain.Permissions
{
    public class PageName
    {
        public const string AdmUser = "User";
        public const string AdmRole = "Role";
        public const string MstCategory = "Category";
        public const string MstProduct = "Product";
        public const string MstCompany = "Company";
        public const string MstEmployee = "Employee";
        public const string MstExpense = "Expense";
        public const string MstActivation = "Activation";
        public const string MstAttendance = "Attendance";
        public const string RptMonthwise = "MonthwiseRpt";
        public const string RptMonthwiseSummaryRpt = "MonthwiseSummaryRpt";
        public const string RptDivisionWiseSummaryRpt = "DivisionWiseSummaryRpt";
        public const string RptDepartmentWiseSummaryRpt = "DepartmentWiseSummaryRpt";
        public const string RptDesignationWiseSummaryRpt = "DesignationWiseSummaryRpt";        
        public const string RptExpenseRpt = "ExpenseRpt";

        public const string Orders = "Orders";
        public const string MstJackpot = "Jackpot";
        public const string MstCoinRecharge = "CoinRecharge";
        public const string MstVehicleType = "VehicleType";
        public const string MstBrandGroup = "BrandGroup";
        public const string MstProductionPlant = "ProductionPlant";
        public const string MstModelFamily = "ModelFamily";
        public const string MstCCGroup = "CCGroup";
        public const string MstPackingType = "PackingType";
        public const string AdmRolePermission = "Permission";
        public const string AdmAuditTrail = "AuditTrail";
        public const string AdmSysSetting = "SystemSetting";
        public const string AdmMailQueue = "MailQueue";
        public const string AdmEmailTemplate = "EmailTemplate";
        public const string MstColor = "Color";
        public const string MstGroupMaster = "GroupMaster";
        public const string MstSubGroupMaster = "SubGroupMaster";
        public const string MstTransporterMaster = "Transporter";
        public const string MstQualityMaster = "Quality";
        public const string MstCompanyMaster = "CompanyMaster";
        public const string MstCompanyFieldMaster = "CompanyFieldMaster";
        public const string MstDbFieldMaster = "DbFieldMaster";
        public const string MstBrokerMaster = "BrokerMaster";
        public const string MstFieldMapping = "FieldMapping";
        public const string MstImportDetail = "ImportDetail";
        public const string MstCustomerMaster = "CustomerMaster";
        public const string MstImporter = "Importer";
        public const string MstContainerType = "ContainerType";
        public const string MstModel = "Model";
        public const string MstContainerModelMapping = "ContainerModelMapping";
        public const string MstPlantModelMapping = "PlantModelMapping";
        public const string MstImporterModelMapping = "ImporterModelMapping";
    }
    public enum PagePermission
    {
        Add = 1, Edit = 2, Delete = 3, View = 4, AddOrEdit = 5, Update = 6
    }
    public enum TokenRole
    {
        Admin = 1, Branch = 2
    }
}
