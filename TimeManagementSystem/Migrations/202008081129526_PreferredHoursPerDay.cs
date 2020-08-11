namespace TimeManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreferredHoursPerDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PreferredHoursPerDay", c => c.Byte(nullable: false, defaultValue: 8));
        }
        
        public override void Down()
        {
        }
    }
}
