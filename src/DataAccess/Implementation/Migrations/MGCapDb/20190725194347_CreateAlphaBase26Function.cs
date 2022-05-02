using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class CreateAlphaBase26Function : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE FUNCTION [dbo].[ConvertNumberToAlpha]( @number AS BIGINT ) RETURNS VARCHAR(15) AS
BEGIN

  DECLARE @result as VARCHAR(15) = '';

  IF @number <= 0
  BEGIN
    SELECT @result = 'A';
	RETURN @result;
  END

  WHILE @number > 0
    SELECT @number -= 1, @result = CHAR( ASCII( 'A' ) + @number % 26 ) + @result, @number /= 26;
  RETURN @result;
END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION [dbo].[ConvertNumberToAlpha]");
        }
    }
}
