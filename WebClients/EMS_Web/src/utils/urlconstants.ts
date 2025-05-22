class UrlConstants {
  //#region
  static loginUrl = "Login/Authenticate";
  static ForgotUrl = "Login/ForgotPassword";
  static ForgotValidateUrl = "Login/ValidateResetPasswordLink";
  static ResetPasswordUrl = "Login/ResetPassword";
  static adLoginUrl = "Login/AdAuthenticate";
  static logoutUrl = "Login/Logout";
  static apiGetLoginUserProfile = "User/get-user-profile";
  static apiLoginUserChangePassword = "User/change-password";
  static apiGetPermissionBasedOnRole = "Permission/get-role-based-permission-data";
  //#endregion
  //#region
  static captchUrl = "Login/GetCaptcha";
  //#endregion
  //#region
  static Permissions = "Permission";
  static apiSavePermissions = "Permission/SavePermission";
  //#endregion
  // company
  static apidropdown = "DropDown/GetBindDropDown";
  //#region  :: User ::
  static apiuser = "User";
  static apiuserlist = "User/List";
  static apiADUserlist = "User/FilterADUser";
  static apiuserupdatestatus = "User/update-status";
  static apiuserupdatelockstatus = "User/update-lockstatus";
  static apiuserprofile = "User/GetUserProfile";
  static apiuserupdateprofile = "User/UpdateProfile";
  static apiuserchangepassword = "User/ChangePassword";
  static apiuserdeactive = "User/DeactiveStatus";
  static apiuseraduserlist = "User/GetADUsers";
  static apiuseraduserphoto = "User/GetAdUserPhoto";
  static apiuseradid = "User/GetAdId";
  static apiBusinessUnitByCountryid = "User/get-BU-by-country";
  //#endregion
  
  //#region  :: Role ::
  static apirole = "Role/";
  static apirolelist = "Role/List";
  static apiroleupdatestatus = "Role/update-status";
  static apirolepagewise = "Role/GetRolePageWise";
  //#endregion

  static apipermissionpagewiserolesave = "Permission";

  //#region :: Permission Record ::
  static apirecordpermissionpagesList = "Permission/GetRecordPermissions";
  static apirecordpermissionpagesSave = "Permission/SetRecordPermissions";
  //#endregion

  //#region  :: SystemSetting ::
  static apisystemsetting = "SystemSetting/";
  static apiesystemsettinglist = "SystemSetting/List";
  static apiSystemSettingByKey = "SystemSetting/get-by-key";

  //#endregion
  
 

  //#region  :: Email Template ::
  static apiemailtemplate = "EmailTemplate/";
  static apiemailtemplatelist = "EmailTemplate/List";
  static apiemailtemplateupdatestatus = "EmailTemplate/update-status";
  static apiemailtemplatenotiupdatestatus = "EmailTemplate/noti-update-status";
  //#endregion
    //#region :: Products Master ::
    static apiProductsMaster = "ProductMasters/";
    static apiProductsMasterList = "ProductMasters/List";
    static apiProductsMasterUpdateStatus = "ProductMasters/update-status";
    static apiProductsMasterUpdateGetOrder = "ProductMasters/get-product-for-order";
    static apiProductsMasterValidateData = "ProductMasters/validate-data";
    static apiProductsMasterImportData = "ProductMasters/import-data";
    
    static apiProductImagesMasters = "ProductImagesMasters/";
    static apiProductImagesMastersList = "ProductImagesMasters/List"; 
    static apiProductImagesMastersUpdateStatus = "ProductImagesMasters/update-thumbnail-status/"; 

  //#endregion
//Dashboard
static apiDashboardMaster = "Dashboard/get-total-summary";

   //#region :: OrderMaster ::
     static apiOrderMaster = "OrderMasters/";
     static apiOrderMasterList = "OrderMasters/List";     
   //#endregion

   //#region :: AttendanceActivation ::
     static apiAttendanceActivation = "AttendanceActivation/";
     static apiAttendanceActivationSearch = "AttendanceActivation/search-employee";
     static apiAttendanceActivationList = "AttendanceActivation/List";     
     static apiAttendanceActivationUpdateStatus = "AttendanceActivation/update-status"; 
   //#endregion

   //#region :: Expense ::
     static apiExpense = "Expense/";
     static apiExpenseList = "Expense/List";
     static apiEmployeeCheckActivation = "Expense/check-activation";
     static apiGetExistingExpense = "Expense/get-existing-expense";
     
   //#endregion
   //#region :: Employee Attendance ::
     static apiEmployeeAttendance = "EmployeeAttendance/";
     static apiEmployeeAttendanceList = "EmployeeAttendance/List";
     static apiGetEmployeeExpense = "EmployeeAttendance/get-employee-expense";
     static apiGetActiveEmployee = "EmployeeAttendance/active-employee-list";
     
   //#endregion


   //#region :: Reports ::
   static apiReports = "Reports/";
   static apiReportsList = "Reports/List";
   static apiReportsDivisionWiseList = "Reports/DivisionWiseList";
   static apiReportsDepartmentWiseList = "Reports/DepartmentWiseList";
   static apiReportsDesignationWiseList = "Reports/DesignationWiseList";
   static apiExpenseReportsList = "Reports/KharchiRpt";
   static apiGetVoucherList = "Reports/get-voucher/";
   
   //Summary Report
   static apiGetMonthwiseSummaryRpt = "Reports/get-monthwise-summary-rpt/";
   static apiGetDivisionWiseSummaryRpt = "Reports/get-divisionwise-summary-rpt/";
   static apiGetDepartmentwiseSummaryRpt = "Reports/get-departmentwise-summary-rpt/";
   static apiGetDesignationWiseSummaryRpt = "Reports/get-designationwise-summary-rpt/";
 //#endregion

  //Order History
   static apiOrderHistoryList = "OrderHistory/List";
   static apiOrderHistoryUpdateStatus = "OrderHistory/update-status";

  //#region :: Employee Master ::
  static apiEmployeeMaster = "Employee/";
  static apiEmployeeMasterList = "Employee/List";
  static apiCompanyMasterUpdateStatus = "Employee/update-status"; 
  //#endregion

  //#region :: Transpoter Master ::
  static apiTransporter = "Transporter/";
  static apiTransporterList = "Transporter/List";
  static apiTransporterUpdateStatus = "Transporter/update-status"; 
  //#endregion

  //#region :: Group Master ::
    static apiGroup = "GroupMaster/";
    static apiGroupList = "GroupMaster/List";
    static apiGroupUpdateStatus = "GroupMaster/update-status"; 
  //#endregion

  //#region :: Sub Group Master ::
    static apiSubGroup = "SubGroup/";
    static apiSubGroupList = "SubGroup/List";
    static apiSubGroupUpdateStatus = "SubGroup/update-status"; 
  //#endregion

  //#region :: Sub Group Master ::
    static apiQuality = "QualityMaster/";
    static apiQualityList = "QualityMaster/List";
    static apiQualityUpdateStatus = "QualityMaster/update-status"; 
  //#endregion

  //#region :: CompanyField Master ::
     static apiCompanyFiled = "CompanyFieldMaster/";
     static apiCompanyFiledList = "CompanyFieldMaster/List";
     static apiCompanyFiledUpdateStatus = "CompanyFieldMaster/update-status"; 
  //#endregion

  //#region :: CompanyField Master ::
          static apiDbFieldMaster = "DbFieldMaster/";
          static apiDbFieldMasterList = "DbFieldMaster/List";
          static apiDbFieldMasterUpdateStatus = "DbFieldMaster/update-status"; 
  //#endregion

  //#region :: Broker Master ::
    static apiBrokerMaster = "BrokerMaster/";
    static apiBrokerMasterList = "BrokerMaster/List";
    static apiBrokerMasterUpdateStatus = "BrokerMaster/update-status"; 
    static apiBrokerMasterGetByCode = "BrokerMaster/get-by-code?id=";
  //#endregion

  //#region :: FieldMapping ::
  static apiFieldMapping = "FieldMapping/";
  static apiFieldMappingList = "FieldMapping/List";
  static apiFieldMappingGetByCompanyId = "FieldMapping/get-field-by-company";
  static apiFieldMappingUpdateStatus = "FieldMapping/update-status"; 
//#endregion

  //#region :: Transpoter Master ::
  static apicustomer = "CustomerMaster/";
  static apicustomerList = "CustomerMaster/List";
  static apicustomerUpdateStatus = "CustomerMaster/update-status"; 
  //#endregion

  //#region :: Importer Mater
  static apiImporterList = "ImportDetail/List";
  static apiImporterDetail = "ImportDetail/Post";
  static apiImporterDetailGetById = "ImportDetail/";
  static apiImporterUpload = "ImportDetail/ImportFile";
   //#endregion
   //#region :: Dispatch
   static apiDispatchDetail = "DispatchDetail/";
   static apiDispatchDetailPost = "DispatchDetail/Post";
   static apiDispatchDetailBulkPost = "DispatchDetail/BulkPost";
   static apiDispatchDetailList = "DispatchDetail/List";
   static apiDispatchDetailViewList = "DispatchDetail/get-dispatch-detail";
   static apiDispatchCustomerList = "DispatchDetail/get-customer-filter";
   //#end region
   
   //#region :: Wellknown Master ::
  static apiWellknownMail = "Report/List";
  //#endregion

  //#region :: CategoryMasters/List
  static apiCategoryMasters = "CategoryMasters/";
  static apiCategoryMastersList = "CategoryMasters/List";
  static apiCategoryMastersUpdateStatus = "CategoryMasters/update-status"; 
  //#endregion

  //#region  :: Audit Log ::
  static readonly apiAuditLog = "AuditLog/";
  static readonly apiAuditLogList = "AuditLog/List";

  //#region  :: Mail Queue ::
  static readonly apiMailQueueList = "MailQueue/List";
  static readonly apiMailQueue = "MailQueue/";

}

export default UrlConstants;
