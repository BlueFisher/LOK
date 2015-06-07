namespace LOK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", "EmailIndex");
            DropIndex("dbo.AspNetUsers", "PhoneNumberIndex");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.AspNetUsers", "PhoneNumber", unique: true, name: "PhoneNumberIndex");
            CreateIndex("dbo.AspNetUsers", "Email", unique: true, name: "EmailIndex");
        }
    }
}
