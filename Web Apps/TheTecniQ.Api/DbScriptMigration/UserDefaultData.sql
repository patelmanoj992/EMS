

------ Role Data ---------------
IF NOT EXISTS (SELECT 1 FROM [EMS_Role] WHERE Id=1)
    BEGIN
        SET IDENTITY_INSERT dbo.[EMS_Role] ON;
        INSERT INTO [dbo].[EMS_Role]([Id],[Name],[IsActive]) VALUES(1,N'Admin',1)
        SET IDENTITY_INSERT dbo.[EMS_Role] OFF;
    END
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[EMS_User] WHERE UserName='Admin')
	INSERT INTO [dbo].[EMS_User]([RoleId],[UserName],[Password],[FirstName],[LastName],[Email],[Mobile],[CreatedBy],[CreatedDate],[IsDeleted],[IsActive],[Is2FA])
     VALUES(1,'Admin','e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7','Admin','Admin','Admin@gmail.com','',1,GETDATE(),0,1,1)
GO
------------ Permission Data---------------------
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Setting')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Setting', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='AuditTrail')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'AuditTrail', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Roles&Permissions')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Roles&Permissions', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Masters')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Masters', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Attendance')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Attendance', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Expense')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Expense', 1)
IF NOT EXISTS (SELECT 1 FROM EMS_Module WHERE ModuleName='Report')
    INSERT [dbo].[EMS_Module] ( [ModuleName], [IsActive]) VALUES ( N'Report', 1)


IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Role')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Roles&Permissions'), 'Role','Role',1,1,'Role',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='User')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Roles&Permissions'), 'User','User',1,1,'User',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='SystemSetting')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Setting'), 'SystemSetting','SystemSetting',1,1,'Settings',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='AuditTrail')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Setting'), 'AuditTrail','AuditTrail',1,1,'Log',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Permission')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Roles&Permissions'), 'Permission','Permission',1,1,'Permission',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='EmailTemplate')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Setting'), 'EmailTemplate','EmailTemplate',1,1,'EmailTemplateMaster',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='MailQueue')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Setting'), 'MailQueue','MailQueue',1,1,'MailQueue',1) 
    TRUNCATE TABLE EMS_Permission
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Employee')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Masters'), 'Employee','Employee',1,1,'Employee',1) 
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Activation')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Attendance'), 'Activation','Activation',1,1,'Activation',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Kharchi')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Expense'), 'Kharchi','Expense',1,1,'Expense',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Attendance')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Attendance'), 'Attendance','Attendance',1,1,'Attendance',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Monthwise')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Monthwise','MonthwiseRpt',1,1,'Report',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='Expense')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Expense','ExpenseRpt',1,1,'Report',1)

DELETE FROM [dbo].[EMS_Page] WHERE PageName IN ('DivisionWise','DepartmentWise','DesignationWise')

IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='MonthwiseSummaryRpt')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Monthwise Summary','MonthwiseSummaryRpt',1,1,'Report',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='DivisionWiseSummaryRpt')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Divisionwise Summary','DivisionWiseSummaryRpt',1,1,'Report',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='DepartmentWiseSummaryRpt')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Departmentwise Summary','DepartmentWiseSummaryRpt',1,1,'Report',1)
IF NOT EXISTS (SELECT 1 FROM [EMS_Page] WHERE PageName='DesignationWiseSummaryRpt')
    INSERT [dbo].[EMS_Page] ( [ModuleId], [PageName], [PageCode], [IsShowMenu], [IsShowSearch], [TableNames], [IsActive]) 
    VALUES ( (SELECT Id FROM EMS_Module WHERE ModuleName='Report'), 'Designationwise Summary','DesignationWiseSummaryRpt',1,1,'Report',1)

    TRUNCATE TABLE EMS_Permission
    ------------ Permission data -------------------

INSERT INTO EMS_Permission(RoleId,PageId,IsAdd,IsDelete,IsEdit,IsView)
SELECT 1,P.Id,1,1,1,1 FROM [EMS_Page] P
INNER jOIN EMS_Module M ON M.Id=P.ModuleId
WHERE P.IsActive=1

-- Insert for setting.product.thumbnail.folder
IF NOT EXISTS (SELECT 1 FROM EMS_Settings WHERE [key] = 'setting.apitokenexpiry')
    INSERT INTO [dbo].[EMS_Settings] ([Key], [Value]) VALUES (N'setting.apitokenexpiry', '1440');
IF NOT EXISTS (SELECT 1 FROM EMS_Settings WHERE [key] = 'setting.mobilegridtotalrow')
    INSERT INTO [dbo].[EMS_Settings] ([Key], [Value]) VALUES (N'setting.mobilegridtotalrow', '20');
