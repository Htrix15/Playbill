﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DbSqlite.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("Models.Places.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Places");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "15/86"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Garry Bar"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Детский город профессий КидБург"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Palazzo"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Palmira Business Club"
                        },
                        new
                        {
                            Id = 6,
                            Name = "PintaHaus"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Saburov-Hall"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Scandy Park"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Star&Mlad"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Station Mir"
                        },
                        new
                        {
                            Id = 11,
                            Name = "«University Hall» (Концертный зал ВГУ)"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Андерсон EVENT площадка"
                        },
                        new
                        {
                            Id = 13,
                            Name = "База отдыха «Егоркины горки»"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Бар ХЛАМ"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Бобровский Районный Дом Культуры"
                        },
                        new
                        {
                            Id = 16,
                            Name = "Бунырево (Тульская область)"
                        },
                        new
                        {
                            Id = 17,
                            Name = "ВГМА им. Н.Н. Бурденко"
                        },
                        new
                        {
                            Id = 18,
                            Name = "Филармония"
                        },
                        new
                        {
                            Id = 19,
                            Name = "Воронежский государственный университет"
                        },
                        new
                        {
                            Id = 20,
                            Name = "Камерный театр"
                        },
                        new
                        {
                            Id = 21,
                            Name = "Воронежский концертный зал"
                        },
                        new
                        {
                            Id = 22,
                            Name = "Воронежский цирк"
                        },
                        new
                        {
                            Id = 23,
                            Name = "Гастрономический ресторан «Москва»"
                        },
                        new
                        {
                            Id = 24,
                            Name = "Городской Дворец культуры"
                        },
                        new
                        {
                            Id = 25,
                            Name = "Дворец Ольденбургских"
                        },
                        new
                        {
                            Id = 26,
                            Name = "Парк Ольденбургских"
                        },
                        new
                        {
                            Id = 27,
                            Name = "Дворец творчества детей и молодежи"
                        },
                        new
                        {
                            Id = 28,
                            Name = "ДК Железнодорожников"
                        },
                        new
                        {
                            Id = 29,
                            Name = "Дом актёра"
                        },
                        new
                        {
                            Id = 30,
                            Name = "ЖД вокзал"
                        },
                        new
                        {
                            Id = 31,
                            Name = "Загородный комплекс \"Villa DaVinci\""
                        },
                        new
                        {
                            Id = 32,
                            Name = "Зеленый театр"
                        },
                        new
                        {
                            Id = 33,
                            Name = "Импровизационный театр «Попкорн-драма»"
                        },
                        new
                        {
                            Id = 34,
                            Name = "Интерлингва"
                        },
                        new
                        {
                            Id = 35,
                            Name = "AURA"
                        },
                        new
                        {
                            Id = 36,
                            Name = "Клуб 12"
                        },
                        new
                        {
                            Id = 37,
                            Name = "Парк Белый колодец"
                        },
                        new
                        {
                            Id = 38,
                            Name = "Лесная Сказка"
                        },
                        new
                        {
                            Id = 39,
                            Name = "МБУК Городской Дворец культуры"
                        },
                        new
                        {
                            Id = 40,
                            Name = "Молодежный центр"
                        },
                        new
                        {
                            Id = 41,
                            Name = "МТС Live Холл"
                        },
                        new
                        {
                            Id = 42,
                            Name = "Музей \"Петровские корабли\""
                        },
                        new
                        {
                            Id = 43,
                            Name = "Набережная"
                        },
                        new
                        {
                            Id = 44,
                            Name = "Jazz Party"
                        },
                        new
                        {
                            Id = 45,
                            Name = "Нелжа Ру"
                        },
                        new
                        {
                            Id = 46,
                            Name = "НИГИЛИСТ"
                        },
                        new
                        {
                            Id = 47,
                            Name = "Никитинский театр"
                        },
                        new
                        {
                            Id = 48,
                            Name = "Новый театр"
                        },
                        new
                        {
                            Id = 49,
                            Name = "Отель Mercure"
                        },
                        new
                        {
                            Id = 50,
                            Name = "Отель Дегас"
                        },
                        new
                        {
                            Id = 51,
                            Name = "1900"
                        },
                        new
                        {
                            Id = 52,
                            Name = "Винзавод"
                        },
                        new
                        {
                            Id = 53,
                            Name = "Ресторан \"Апраксин\""
                        },
                        new
                        {
                            Id = 54,
                            Name = "ресторан-бар \"Коптильня\""
                        },
                        new
                        {
                            Id = 55,
                            Name = "Ресторан «Форт»"
                        },
                        new
                        {
                            Id = 56,
                            Name = "Рок-бар \"DIESEL\""
                        },
                        new
                        {
                            Id = 57,
                            Name = "Руки ВВерх! Бар"
                        },
                        new
                        {
                            Id = 58,
                            Name = "Сто Ручьёв"
                        },
                        new
                        {
                            Id = 59,
                            Name = "Театр драмы им. А. Кольцова"
                        },
                        new
                        {
                            Id = 60,
                            Name = "Театр Кот"
                        },
                        new
                        {
                            Id = 61,
                            Name = "Театр кукол им. В.А. Вольховского"
                        },
                        new
                        {
                            Id = 62,
                            Name = "Театр оперы и балета"
                        },
                        new
                        {
                            Id = 63,
                            Name = "Театр юного зрителя"
                        },
                        new
                        {
                            Id = 64,
                            Name = "«Тулиновъ Дом»"
                        },
                        new
                        {
                            Id = 65,
                            Name = "Усадьба \"Отрада\" (Калужская обл.)"
                        },
                        new
                        {
                            Id = 66,
                            Name = "Усадьба Скорняково-Архангельское"
                        },
                        new
                        {
                            Id = 67,
                            Name = "Хохольский центр развития культуры и туризма"
                        },
                        new
                        {
                            Id = 68,
                            Name = "Центральный Стадион Профсоюзов"
                        },
                        new
                        {
                            Id = 69,
                            Name = "Центр Галереи Чижова"
                        },
                        new
                        {
                            Id = 70,
                            Name = "ЛДС \"Юбилейный\""
                        },
                        new
                        {
                            Id = 71,
                            Name = "ЯР Hotel&SPA"
                        },
                        new
                        {
                            Id = 72,
                            Name = "Музей-заповедник Дивногорье"
                        },
                        new
                        {
                            Id = 73,
                            Name = "Художественный музей им. Крамского"
                        },
                        new
                        {
                            Id = 74,
                            Name = "Стадион технических видов спорта Воронеж-Ринг"
                        },
                        new
                        {
                            Id = 75,
                            Name = "Портал на Фридриха Энгельса"
                        },
                        new
                        {
                            Id = 76,
                            Name = "Quest Brothers на Московском"
                        },
                        new
                        {
                            Id = 77,
                            Name = "Quest Brothers на Республиканской"
                        },
                        new
                        {
                            Id = 78,
                            Name = "Воронежская Филармония",
                            ParentId = 18
                        },
                        new
                        {
                            Id = 79,
                            Name = "Воронежская филармония",
                            ParentId = 18
                        },
                        new
                        {
                            Id = 80,
                            Name = "Aura Arena Hall",
                            ParentId = 35
                        },
                        new
                        {
                            Id = 81,
                            Name = "«Palazzo»",
                            ParentId = 4
                        },
                        new
                        {
                            Id = 82,
                            Name = "Palazzo (ex. Arena Hall)",
                            ParentId = 4
                        },
                        new
                        {
                            Id = 83,
                            Name = "Дворец Культуры Железнодорожников",
                            ParentId = 28
                        },
                        new
                        {
                            Id = 84,
                            Name = "ДК железнодорожников",
                            ParentId = 28
                        },
                        new
                        {
                            Id = 85,
                            Name = "Сто ручьёв",
                            ParentId = 58
                        },
                        new
                        {
                            Id = 86,
                            Name = "Форт",
                            ParentId = 55
                        },
                        new
                        {
                            Id = 87,
                            Name = "Театр «Кот»",
                            ParentId = 60
                        },
                        new
                        {
                            Id = 88,
                            Name = "Театр Оперы и Балета",
                            ParentId = 62
                        },
                        new
                        {
                            Id = 89,
                            Name = "Воронежский государственный академический театр драмы им. А. Кольцова",
                            ParentId = 59
                        },
                        new
                        {
                            Id = 90,
                            Name = "Воронежский Дом актёра им. Л. Кравцовой",
                            ParentId = 29
                        },
                        new
                        {
                            Id = 91,
                            Name = "Театр «Дом Актера»",
                            ParentId = 29
                        },
                        new
                        {
                            Id = 92,
                            Name = "Дом Актера",
                            ParentId = 29
                        },
                        new
                        {
                            Id = 93,
                            Name = "МТС Live Холл (ex. Event Hall)",
                            ParentId = 41
                        },
                        new
                        {
                            Id = 94,
                            Name = "Музей- заповедник Дивногорье мастер классы",
                            ParentId = 72
                        },
                        new
                        {
                            Id = 95,
                            Name = "Музей- заповедник Дивногорье",
                            ParentId = 72
                        },
                        new
                        {
                            Id = 96,
                            Name = "Воронежский государственный театр юного зрителя",
                            ParentId = 63
                        },
                        new
                        {
                            Id = 97,
                            Name = "Воронежский Камерный театр",
                            ParentId = 20
                        },
                        new
                        {
                            Id = 98,
                            Name = "Пространство \"Винзавод\"",
                            ParentId = 52
                        },
                        new
                        {
                            Id = 99,
                            Name = "Железнодорожный вокзал",
                            ParentId = 30
                        },
                        new
                        {
                            Id = 100,
                            Name = "МБУК «Боринский ЦКРиД»"
                        },
                        new
                        {
                            Id = 101,
                            Name = "Pinta Haus"
                        },
                        new
                        {
                            Id = 102,
                            Name = "«На крыше под куполом» JAZZPARTY",
                            ParentId = 44
                        },
                        new
                        {
                            Id = 103,
                            Name = "Площадка 1900",
                            ParentId = 51
                        },
                        new
                        {
                            Id = 104,
                            Name = "Diesel",
                            ParentId = 56
                        });
                });

            modelBuilder.Entity("Models.Places.Place", b =>
                {
                    b.HasOne("Models.Places.Place", "Parent")
                        .WithMany("Synonyms")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Models.Places.Place", b =>
                {
                    b.Navigation("Synonyms");
                });
#pragma warning restore 612, 618
        }
    }
}
