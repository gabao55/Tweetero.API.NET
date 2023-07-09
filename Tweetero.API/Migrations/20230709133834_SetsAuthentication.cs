using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tweetero.API.Migrations
{
    /// <inheritdoc />
    public partial class SetsAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tweets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "https://www.racoesreis.com.br/wordpress/wp-content/uploads/cachorro-origem3.jpg", "AQAAAAEAACcQAAAAEBfUqsmn6PYi0kBuwDKnFIRi4Jp9lGOg04DDOB7pnSOQLPQQLsqNkyWJfgjsb6B2gw==", "test" },
                    { 2, "https://www.racoesreis.com.br/wordpress/wp-content/uploads/cachorro-origem3.jpg", "AQAAAAEAACcQAAAAEEInRuW4i8YHtX6v2hCLT1aCfK8u0+1yfdO+v4r/FVh5D0RjucaijvArS8jdWTZuWw==", "test2" }
                });

            migrationBuilder.InsertData(
                table: "Tweets",
                columns: new[] { "Id", "Message", "UserId" },
                values: new object[,]
                {
                    { 1, "I love crunchy lettuce!", 1 },
                    { 2, "I hate when I get alone... :(", 1 },
                    { 3, "Testing second user", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserId",
                table: "Tweets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tweets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
