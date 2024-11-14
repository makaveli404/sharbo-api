using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharboAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyNewDbStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Situations",
                table: "Situations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Memes",
                table: "Memes");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Memes",
                newName: "ImagePath");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Situations",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Situations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Quotes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Quotes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Memes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Memes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedById",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Entries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GroupParticipants",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupParticipants", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupParticipants_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Situations_EntryId",
                table: "Situations",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_EntryId",
                table: "Quotes",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Memes_EntryId",
                table: "Memes",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedById",
                table: "Groups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LastModifiedById",
                table: "Groups",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_CreatedById",
                table: "Entries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GroupParticipants_UserId",
                table: "GroupParticipants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Users_CreatedById",
                table: "Entries",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_CreatedById",
                table: "Groups",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_LastModifiedById",
                table: "Groups",
                column: "LastModifiedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memes_Entries_EntryId",
                table: "Memes",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Entries_EntryId",
                table: "Quotes",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Situations_Entries_EntryId",
                table: "Situations",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Users_CreatedById",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_CreatedById",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_LastModifiedById",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Memes_Entries_EntryId",
                table: "Memes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Entries_EntryId",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Situations_Entries_EntryId",
                table: "Situations");

            migrationBuilder.DropTable(
                name: "GroupParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Situations_EntryId",
                table: "Situations");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_EntryId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Memes_EntryId",
                table: "Memes");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CreatedById",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LastModifiedById",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Entries_CreatedById",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LastModifiedById",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Entries");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Memes",
                newName: "Image");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Situations",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Situations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Quotes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Quotes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Memes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EntryId",
                table: "Memes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Situations",
                table: "Situations",
                column: "EntryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quotes",
                table: "Quotes",
                column: "EntryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Memes",
                table: "Memes",
                column: "EntryId");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });
        }
    }
}
