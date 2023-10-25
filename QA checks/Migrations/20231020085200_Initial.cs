using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_checks.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdersNumber = table.Column<long>(type: "bigint", nullable: false),
                    OrdersName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QAchecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrdersNumber = table.Column<long>(type: "bigint", nullable: false),
                    Pasteryzacja = table.Column<int>(type: "int", nullable: false),
                    PasteryzacjaKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CiałaObce = table.Column<int>(type: "int", nullable: false),
                    CiałaObceKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataOpakowania = table.Column<int>(type: "int", nullable: false),
                    DataOpakowaniaKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Receptura = table.Column<int>(type: "int", nullable: false),
                    RecepturaKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetalDetektor = table.Column<int>(type: "int", nullable: false),
                    MetalDetektorKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opakowanie = table.Column<int>(type: "int", nullable: false),
                    OpakowanieKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestWodny = table.Column<int>(type: "int", nullable: false),
                    TestKomentarz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lepkość = table.Column<float>(type: "real", nullable: false),
                    Ekstrakt = table.Column<float>(type: "real", nullable: false),
                    Ph = table.Column<float>(type: "real", nullable: false),
                    Temperatura = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QAchecks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "QAchecks");
        }
    }
}
