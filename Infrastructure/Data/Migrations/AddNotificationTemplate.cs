using Microsoft.EntityFrameworkCore.Migrations;

namespace XianYu.API.Infrastructure.Data.Migrations;

public partial class AddNotificationTemplate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "NotificationTemplates",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_NotificationTemplates_Code",
            table: "NotificationTemplates",
            column: "Code",
            unique: true);

        // 添加默认模板
        migrationBuilder.InsertData(
            table: "NotificationTemplates",
            columns: new[] { "Name", "Code", "Type", "Content", "Description", "IsActive", "CreatedAt" },
            values: new object[]
            {
                "订单发货邮件通知",
                "ORDER_DELIVERY_EMAIL",
                0, // Email
                @"<html><body><h2>发货通知</h2><p>尊敬的用户：</p><p>您的订单已发货，详情如下：</p><ul><li>订单号：{OrderNumber}</li><li>商品名称：{ProductName}</li><li>发货方式：{DeliveryMethod}</li><li>发货内容：{DeliveryContent}</li><li>发货时间：{DeliveryTime}</li></ul><p>感谢您的购买！</p></body></html>",
                "订单发货邮件通知模板",
                true,
                DateTime.UtcNow
            });

        migrationBuilder.InsertData(
            table: "NotificationTemplates",
            columns: new[] { "Name", "Code", "Type", "Content", "Description", "IsActive", "CreatedAt" },
            values: new object[]
            {
                "订单发货短信通知",
                "ORDER_DELIVERY_SMS",
                1, // SMS
                "您的订单{OrderNumber}已发货，商品：{ProductName}，发货方式：{DeliveryMethod}，发货内容：{DeliveryContent}。",
                "订单发货短信通知模板",
                true,
                DateTime.UtcNow
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "NotificationTemplates");
    }
} 