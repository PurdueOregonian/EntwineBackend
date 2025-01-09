using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EntwineBackend.Migrations
{
    /// <inheritdoc />
    public partial class Interests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<string>(type: "text", nullable: true),
                    UserIds = table.Column<List<int>>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categories = table.Column<List<int>>(type: "integer[]", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InterestCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Creativity" },
                    { 2, "Nature" },
                    { 3, "Technology" },
                    { 4, "Fitness & Sports" },
                    { 5, "Travel" },
                    { 6, "Music" }
                });

            migrationBuilder.InsertData(
                table: "Interests",
                columns: new[] { "Id", "Categories", "Name" },
                values: new object[,]
                {
                    { 1, new List<int> { 1 }, "Digital Art" },
                    { 2, new List<int> { 1 }, "Photography" },
                    { 3, new List<int> { 1 }, "Painting" },
                    { 4, new List<int> { 1 }, "Knitting" },
                    { 5, new List<int> { 1, 6 }, "Music Composition" },
                    { 6, new List<int> { 1, 6 }, "Cooking" },
                    { 7, new List<int> { 1, 6 }, "Woodworking" },
                    { 8, new List<int> { 2, 4 }, "Hiking" },
                    { 9, new List<int> { 2 }, "Gardening" },
                    { 10, new List<int> { 1, 3 }, "Programming" },
                    { 11, new List<int> { 1, 3 }, "Hardware Assembly" },
                    { 12, new List<int> { 4 }, "Basketball" },
                    { 13, new List<int> { 4 }, "Soccer" },
                    { 14, new List<int> { 5 }, "Backpacking" },
                    { 15, new List<int> { 6 }, "Jazz" },
                    { 16, new List<int> { 6 }, "Pop" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communities_Id",
                table: "Communities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterestCategories_Id",
                table: "InterestCategories",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interests_Id",
                table: "Interests",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "InterestCategories");

            migrationBuilder.DropTable(
                name: "Interests");
        }
    }
}
