using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyInvoiceApp_API.Migrations
{
    /// <inheritdoc />
    public partial class RenameClientModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Company_Name",
                table: "Clients",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Clients",
                newName: "Company_Name");
        }
    }
}
