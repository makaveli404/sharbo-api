using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharboAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProblemWithManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Entries_EntryId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_EntryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EntryId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "EntryParticipants",
                columns: table => new
                {
                    EntriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryParticipants", x => new { x.EntriesId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_EntryParticipants_Entries_EntriesId",
                        column: x => x.EntriesId,
                        principalTable: "Entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryParticipants_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntryParticipants_ParticipantsId",
                table: "EntryParticipants",
                column: "ParticipantsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryParticipants");

            migrationBuilder.AddColumn<int>(
                name: "EntryId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EntryId",
                table: "Users",
                column: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Entries_EntryId",
                table: "Users",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id");
        }
    }
}
