using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DbSqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Places_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Places",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Places",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[,]
                {
                    { 1, "15/86", null },
                    { 2, "Garry Bar", null },
                    { 3, "Детский город профессий КидБург", null },
                    { 4, "Palazzo", null },
                    { 5, "Palmira Business Club", null },
                    { 6, "PintaHaus", null },
                    { 7, "Saburov-Hall", null },
                    { 8, "Scandy Park", null },
                    { 9, "Star&Mlad", null },
                    { 10, "Station Mir", null },
                    { 11, "«University Hall» (Концертный зал ВГУ)", null },
                    { 12, "Андерсон EVENT площадка", null },
                    { 13, "База отдыха «Егоркины горки»", null },
                    { 14, "Бар ХЛАМ", null },
                    { 15, "Бобровский Районный Дом Культуры", null },
                    { 16, "Бунырево (Тульская область)", null },
                    { 17, "ВГМА им. Н.Н. Бурденко", null },
                    { 18, "Филармония", null },
                    { 19, "Воронежский государственный университет", null },
                    { 20, "Камерный театр", null },
                    { 21, "Воронежский концертный зал", null },
                    { 22, "Воронежский цирк", null },
                    { 23, "Гастрономический ресторан «Москва»", null },
                    { 24, "Городской Дворец культуры", null },
                    { 25, "Дворец Ольденбургских", null },
                    { 26, "Парк Ольденбургских", null },
                    { 27, "Дворец творчества детей и молодежи", null },
                    { 28, "ДК Железнодорожников", null },
                    { 29, "Дом актёра", null },
                    { 30, "ЖД вокзал", null },
                    { 31, "Загородный комплекс \"Villa DaVinci\"", null },
                    { 32, "Зеленый театр", null },
                    { 33, "Импровизационный театр «Попкорн-драма»", null },
                    { 34, "Интерлингва", null },
                    { 35, "AURA", null },
                    { 36, "Клуб 12", null },
                    { 37, "Парк Белый колодец", null },
                    { 38, "Лесная Сказка", null },
                    { 39, "МБУК Городской Дворец культуры", null },
                    { 40, "Молодежный центр", null },
                    { 41, "МТС Live Холл", null },
                    { 42, "Музей \"Петровские корабли\"", null },
                    { 43, "Набережная", null },
                    { 44, "Jazz Party", null },
                    { 45, "Нелжа Ру", null },
                    { 46, "НИГИЛИСТ", null },
                    { 47, "Никитинский театр", null },
                    { 48, "Новый театр", null },
                    { 49, "Отель Mercure", null },
                    { 50, "Отель Дегас", null },
                    { 51, "1900", null },
                    { 52, "Винзавод", null },
                    { 53, "Ресторан \"Апраксин\"", null },
                    { 54, "ресторан-бар \"Коптильня\"", null },
                    { 55, "Ресторан «Форт»", null },
                    { 56, "Рок-бар \"DIESEL\"", null },
                    { 57, "Руки ВВерх! Бар", null },
                    { 58, "Сто Ручьёв", null },
                    { 59, "Театр драмы им. А. Кольцова", null },
                    { 60, "Театр Кот", null },
                    { 61, "Театр кукол им. В.А. Вольховского", null },
                    { 62, "Театр оперы и балета", null },
                    { 63, "Театр юного зрителя", null },
                    { 64, "«Тулиновъ Дом»", null },
                    { 65, "Усадьба \"Отрада\" (Калужская обл.)", null },
                    { 66, "Усадьба Скорняково-Архангельское", null },
                    { 67, "Хохольский центр развития культуры и туризма", null },
                    { 68, "Центральный Стадион Профсоюзов", null },
                    { 69, "Центр Галереи Чижова", null },
                    { 70, "ЛДС \"Юбилейный\"", null },
                    { 71, "ЯР Hotel&SPA", null },
                    { 72, "Музей-заповедник Дивногорье", null },
                    { 73, "Художественный музей им. Крамского", null },
                    { 74, "Стадион технических видов спорта Воронеж-Ринг", null },
                    { 75, "Портал на Фридриха Энгельса", null },
                    { 76, "Quest Brothers на Московском", null },
                    { 77, "Quest Brothers на Республиканской", null },
                    { 100, "МБУК «Боринский ЦКРиД»", null },
                    { 101, "Pinta Haus", null },
                    { 78, "Воронежская Филармония", 18 },
                    { 79, "Воронежская филармония", 18 },
                    { 80, "Aura Arena Hall", 35 },
                    { 81, "«Palazzo»", 4 },
                    { 82, "Palazzo (ex. Arena Hall)", 4 },
                    { 83, "Дворец Культуры Железнодорожников", 28 },
                    { 84, "ДК железнодорожников", 28 },
                    { 85, "Сто ручьёв", 58 },
                    { 86, "Форт", 55 },
                    { 87, "Театр «Кот»", 60 },
                    { 88, "Театр Оперы и Балета", 62 },
                    { 89, "Воронежский государственный академический театр драмы им. А. Кольцова", 59 },
                    { 90, "Воронежский Дом актёра им. Л. Кравцовой", 29 },
                    { 91, "Театр «Дом Актера»", 29 },
                    { 92, "Дом Актера", 29 },
                    { 93, "МТС Live Холл (ex. Event Hall)", 41 },
                    { 94, "Музей- заповедник Дивногорье мастер классы", 72 },
                    { 95, "Музей- заповедник Дивногорье", 72 },
                    { 96, "Воронежский государственный театр юного зрителя", 63 },
                    { 97, "Воронежский Камерный театр", 20 },
                    { 98, "Пространство \"Винзавод\"", 52 },
                    { 99, "Железнодорожный вокзал", 30 },
                    { 102, "«На крыше под куполом» JAZZPARTY", 44 },
                    { 103, "Площадка 1900", 51 },
                    { 104, "Diesel", 56 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Places_ParentId",
                table: "Places",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
