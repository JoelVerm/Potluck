using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Example.Migrations
{
    /// <inheritdoc />
    public partial class UserHomeOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "HouseId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users",
                column: "HouseId",
                principalTable: "Houses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "HouseId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users",
                column: "HouseId",
                principalTable: "Houses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
