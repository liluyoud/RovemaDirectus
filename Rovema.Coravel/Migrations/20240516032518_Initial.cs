﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rovema.Coravel.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coravel_JobHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TypeFullPath = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Failed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    StackTrace = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coravel_JobHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coravel_ScheduledJobHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TypeFullPath = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Failed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    StackTrace = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coravel_ScheduledJobHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coravel_ScheduledJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvocableFullPath = table.Column<string>(type: "TEXT", nullable: true),
                    CronExpression = table.Column<string>(type: "TEXT", nullable: true),
                    Frequency = table.Column<string>(type: "TEXT", nullable: true),
                    Days = table.Column<string>(type: "TEXT", nullable: true),
                    PreventOverlapping = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimeZoneInfo = table.Column<string>(type: "TEXT", nullable: true),
                    RunOnDedicatedThread = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coravel_ScheduledJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coravel_JobHistory_EndedAt",
                table: "Coravel_JobHistory",
                column: "EndedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Coravel_ScheduledJobHistory_EndedAt",
                table: "Coravel_ScheduledJobHistory",
                column: "EndedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Coravel_ScheduledJobs_Active",
                table: "Coravel_ScheduledJobs",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Coravel_ScheduledJobs_InvocableFullPath",
                table: "Coravel_ScheduledJobs",
                column: "InvocableFullPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coravel_JobHistory");

            migrationBuilder.DropTable(
                name: "Coravel_ScheduledJobHistory");

            migrationBuilder.DropTable(
                name: "Coravel_ScheduledJobs");
        }
    }
}
