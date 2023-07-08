using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tweetero.API.Migrations
{
    /// <inheritdoc />
    public partial class NewSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Password", "Username" },
                values: new object[] { 2, "https://www.racoesreis.com.br/wordpress/wp-content/uploads/cachorro-origem3.jpg", "123456", "test2" });

            migrationBuilder.InsertData(
                table: "Tweets",
                columns: new[] { "Id", "Message", "UserId" },
                values: new object[] { 3, "Testing second user", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tweets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
