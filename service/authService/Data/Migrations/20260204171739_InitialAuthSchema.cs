using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuthSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auth_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    first_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    last_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "user"),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auth_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.token);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_auth_users_user_id",
                        column: x => x.user_id,
                        principalTable: "auth_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auth_users_email",
                table: "auth_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_auth_users_username",
                table: "auth_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "auth_users");
        }
    }
}
