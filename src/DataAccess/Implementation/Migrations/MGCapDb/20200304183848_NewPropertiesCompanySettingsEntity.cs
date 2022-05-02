using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class NewPropertiesCompanySettingsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FederalInsuranceContributionsAct",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FederalUnemploymentTaxAct",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GeneralLedger",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Medicare",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StateUnemploymentInsurance",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WorkersCompensation",
                table: "CompanySettings",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FederalInsuranceContributionsAct",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "FederalUnemploymentTaxAct",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "GeneralLedger",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "Medicare",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "StateUnemploymentInsurance",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "WorkersCompensation",
                table: "CompanySettings");
        }
    }
}
