using TheTecniQ.Data.Migrations;

namespace TheTecniQ.Data.Migration
{
    [MigrationInfo("2025-05-22 17:55:00", "Add Model")]
    public class StoreProcedureMigration : FluentMigrator.Migration
    {
        public override void Up()
        {
           Execute.Script("DbScriptMigration/UserDefaultData.sql");
           //Execute.Script("DbScriptMigration/EMS_GetPlanningWisePendingOrder.sql");
           //Execute.Script("DbScriptMigration/EMS_GetDeyingProductionPlanList.sql");
        }

        public override void Down()
        {
        }

    }
}
