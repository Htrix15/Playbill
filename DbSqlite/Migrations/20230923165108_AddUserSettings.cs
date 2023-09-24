using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbSqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    ExcludeBillboards = table.Column<string>(type: "TEXT", nullable: true),
                    ExcludeEventTypes = table.Column<string>(type: "TEXT", nullable: true),
                    ExcludeDaysOfWeek = table.Column<string>(type: "TEXT", nullable: true),
                    ExcludePlacesIds = table.Column<string>(type: "TEXT", nullable: true),
                    AddHolidays = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
