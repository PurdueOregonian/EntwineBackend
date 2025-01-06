using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friends5___Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestsToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "Interests",
                table: "Profiles",
                type: "integer[]",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 1,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 2,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 3,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 4,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 5,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 6,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 7,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 8,
                column: "Categories",
                value: new List<int> { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 9,
                column: "Categories",
                value: new List<int> { 2 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 10,
                column: "Categories",
                value: new List<int> { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 11,
                column: "Categories",
                value: new List<int> { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 12,
                column: "Categories",
                value: new List<int> { 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 13,
                column: "Categories",
                value: new List<int> { 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 14,
                column: "Categories",
                value: new List<int> { 5 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 15,
                column: "Categories",
                value: new List<int> { 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 16,
                column: "Categories",
                value: new List<int> { 6 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Profiles");

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 1,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 2,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 3,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 4,
                column: "Categories",
                value: new List<int> { 1 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 5,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 6,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 7,
                column: "Categories",
                value: new List<int> { 1, 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 8,
                column: "Categories",
                value: new List<int> { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 9,
                column: "Categories",
                value: new List<int> { 2 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 10,
                column: "Categories",
                value: new List<int> { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 11,
                column: "Categories",
                value: new List<int> { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 12,
                column: "Categories",
                value: new List<int> { 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 13,
                column: "Categories",
                value: new List<int> { 4 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 14,
                column: "Categories",
                value: new List<int> { 5 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 15,
                column: "Categories",
                value: new List<int> { 6 });

            migrationBuilder.UpdateData(
                table: "Interests",
                keyColumn: "Id",
                keyValue: 16,
                column: "Categories",
                value: new List<int> { 6 });
        }
    }
}
