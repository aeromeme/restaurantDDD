using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Categories",
               columns: new[] { "Id", "Name", "Description" },
               values: new object[,]
               {
                    { Guid.NewGuid(), "Beverages", "Drinks and refreshments" },
                    { Guid.NewGuid(), "Appetizers", "Starters and small plates" },
                    { Guid.NewGuid(), "Main Courses", "Main dishes" },
                    { Guid.NewGuid(), "Desserts", "Sweet treats" }
               }
           );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
