using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPark.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSlotRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Booking__SlotId__59063A47",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_SlotId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "Booking");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Slots",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Slots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_BookingId",
                table: "Slots",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK__Slots__BookingId__5441852A",
                table: "Slots",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Slots__BookingId__5441852A",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_BookingId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Slots");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Slots",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "Booking",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Booking_SlotId",
                table: "Booking",
                column: "SlotId");

            migrationBuilder.AddForeignKey(
                name: "FK__Booking__SlotId__59063A47",
                table: "Booking",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id");
        }
    }
}
