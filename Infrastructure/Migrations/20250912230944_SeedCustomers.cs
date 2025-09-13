using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "Name", "Email" },
                values: new object[,]
                {
                    { Guid.Parse("11111111-1111-1111-1111-111111111111"), "John Doe", "john.doe@example.com" },
                    { Guid.Parse("22222222-2222-2222-2222-222222222222"), "Jane Smith", "jane.smith@example.com" },
                    { Guid.Parse("33333333-3333-3333-3333-333333333333"), "Alice Johnson", "alice.johnson@example.com" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
