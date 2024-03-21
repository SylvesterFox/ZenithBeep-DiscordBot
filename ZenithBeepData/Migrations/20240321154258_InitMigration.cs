using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ZenithBeepData.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guildName = table.Column<string>(type: "text", nullable: false),
                    Lang = table.Column<string>(type: "text", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    MusicChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    MusicChannelName = table.Column<string>(type: "text", nullable: true),
                    LastMessageStatusId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    NextTrack = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TrackCount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CurrentTrack = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    IsPlaying = table.Column<bool>(type: "boolean", nullable: false),
                    LeaveAfterQueue = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_channel = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_name = table.Column<string>(type: "text", nullable: false),
                    limit = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TempRooms",
                columns: table => new
                {
                    userId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channelRoomId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempRooms", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "CachedUser",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ModelGuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CachedUser_Guild_ModelGuildId",
                        column: x => x.ModelGuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    messageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    roleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    setEmoji = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomsLobbys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    lobby_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomsLobbys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomsLobbys_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArchivedTracks",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DateMessageCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Position = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TrackString = table.Column<string>(type: "text", nullable: false),
                    RequestedById = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    CachedUserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedTracks_CachedUser_CachedUserId",
                        column: x => x.CachedUserId,
                        principalTable: "CachedUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchivedTracks_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildQueuePlaylists",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    PlaylistSongConut = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildQueuePlaylists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildQueuePlaylists_CachedUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CachedUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuildQueueItems",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Position = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TrackString = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    DeletedReason = table.Column<string>(type: "text", nullable: true),
                    RequestedById = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    PlaylistId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildQueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildQueueItems_CachedUser_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "CachedUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuildQueueItems_CachedUser_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "CachedUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuildQueueItems_GuildQueuePlaylists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "GuildQueuePlaylists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuildQueueItems_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTracks_CachedUserId",
                table: "ArchivedTracks",
                column: "CachedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTracks_GuildId",
                table: "ArchivedTracks",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedUser_ModelGuildId",
                table: "CachedUser",
                column: "ModelGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQueueItems_DeletedById",
                table: "GuildQueueItems",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQueueItems_GuildId",
                table: "GuildQueueItems",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQueueItems_PlaylistId",
                table: "GuildQueueItems",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQueueItems_RequestedById",
                table: "GuildQueueItems",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_GuildQueuePlaylists_CreatedById",
                table: "GuildQueuePlaylists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId",
                table: "Roles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomsLobbys_GuildId",
                table: "RoomsLobbys",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTracks");

            migrationBuilder.DropTable(
                name: "GuildQueueItems");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomsLobbys");

            migrationBuilder.DropTable(
                name: "TempRooms");

            migrationBuilder.DropTable(
                name: "GuildQueuePlaylists");

            migrationBuilder.DropTable(
                name: "CachedUser");

            migrationBuilder.DropTable(
                name: "Guild");
        }
    }
}
