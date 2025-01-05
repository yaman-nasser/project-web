using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_web.Migrations
{
    /// <inheritdoc />
    public partial class like : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedProduct_users_UserId",
                table: "LikedProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikedProduct",
                table: "LikedProduct");

            migrationBuilder.RenameTable(
                name: "LikedProduct",
                newName: "LikedProducts");

            migrationBuilder.RenameIndex(
                name: "IX_LikedProduct_UserId",
                table: "LikedProducts",
                newName: "IX_LikedProducts_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikedProducts",
                table: "LikedProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LikedProducts_ProductId",
                table: "LikedProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedProducts_products_ProductId",
                table: "LikedProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedProducts_users_UserId",
                table: "LikedProducts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedProducts_products_ProductId",
                table: "LikedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_LikedProducts_users_UserId",
                table: "LikedProducts");

            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikedProducts",
                table: "LikedProducts");

            migrationBuilder.DropIndex(
                name: "IX_LikedProducts_ProductId",
                table: "LikedProducts");

            migrationBuilder.RenameTable(
                name: "LikedProducts",
                newName: "LikedProduct");

            migrationBuilder.RenameIndex(
                name: "IX_LikedProducts_UserId",
                table: "LikedProduct",
                newName: "IX_LikedProduct_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikedProduct",
                table: "LikedProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedProduct_users_UserId",
                table: "LikedProduct",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
