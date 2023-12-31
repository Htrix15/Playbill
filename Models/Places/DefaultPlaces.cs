﻿
namespace Models.Places;

public static class DefaultPlaces
{
    public static List<Place> Places => new List<Place> {   new Place() { Id = 1, Name = "15/86", },
            new Place() { Id = 2, Name = "Garry Bar", },
            new Place() { Id = 3, Name = "Детский город профессий КидБург", },
            new Place() { Id = 4, Name = "Palazzo", },
            new Place() { Id = 5, Name = "Palmira Business Club", },
            new Place() { Id = 6, Name = "PintaHaus", },
            new Place() { Id = 7, Name = "Saburov-Hall", },
            new Place() { Id = 8, Name = "Scandy Park", },
            new Place() { Id = 9, Name = "Star&Mlad", },
            new Place() { Id = 10, Name = "Station Mir", },
            new Place() { Id = 11, Name = "«University Hall» (Концертный зал ВГУ)", },
            new Place() { Id = 12, Name = "Андерсон EVENT площадка", },
            new Place() { Id = 13, Name = "База отдыха «Егоркины горки»", },
            new Place() { Id = 14, Name = "Бар ХЛАМ", },
            new Place() { Id = 15, Name = "Бобровский Районный Дом Культуры", },
            new Place() { Id = 16, Name = "Бунырево (Тульская область)", },
            new Place() { Id = 17, Name = "ВГМА им. Н.Н. Бурденко", },
            new Place() { Id = 18, Name = "Филармония", },
            new Place() { Id = 19, Name = "Воронежский государственный университет", },
            new Place() { Id = 20, Name = "Камерный театр", },
            new Place() { Id = 21, Name = "Воронежский концертный зал", },
            new Place() { Id = 22, Name = "Воронежский цирк", },
            new Place() { Id = 23, Name = "Гастрономический ресторан «Москва»", },
            new Place() { Id = 24, Name = "Городской Дворец культуры", },
            new Place() { Id = 25, Name = "Дворец Ольденбургских", },
            new Place() { Id = 26, Name = "Парк Ольденбургских", },
            new Place() { Id = 27, Name = "Дворец творчества детей и молодежи", },
            new Place() { Id = 28, Name = "ДК Железнодорожников", },
            new Place() { Id = 29, Name = "Дом актёра", },
            new Place() { Id = 30, Name = "ЖД вокзал", },
            new Place() { Id = 31, Name = "Загородный комплекс \"Villa DaVinci\"", },
            new Place() { Id = 32, Name = "Зеленый театр", },
            new Place() { Id = 33, Name = "Импровизационный театр «Попкорн-драма»", },
            new Place() { Id = 34, Name = "Интерлингва", },
            new Place() { Id = 35, Name = "AURA", },
            new Place() { Id = 36, Name = "Клуб 12", },
            new Place() { Id = 37, Name = "Парк Белый колодец", },
            new Place() { Id = 38, Name = "Лесная Сказка", },
            new Place() { Id = 39, Name = "МБУК Городской Дворец культуры", },
            new Place() { Id = 40, Name = "Молодежный центр", },
            new Place() { Id = 41, Name = "МТС Live Холл", },
            new Place() { Id = 42, Name = "Музей \"Петровские корабли\"", },
            new Place() { Id = 43, Name = "Набережная", },
            new Place() { Id = 44, Name = "Jazz Party", },
            new Place() { Id = 45, Name = "Нелжа Ру", },
            new Place() { Id = 46, Name = "НИГИЛИСТ", },
            new Place() { Id = 47, Name = "Никитинский театр", },
            new Place() { Id = 48, Name = "Новый театр", },
            new Place() { Id = 49, Name = "Отель Mercure", },
            new Place() { Id = 50, Name = "Отель Дегас", },
            new Place() { Id = 51, Name = "1900", },
            new Place() { Id = 52, Name = "Винзавод", },
            new Place() { Id = 53, Name = "Ресторан \"Апраксин\"", },
            new Place() { Id = 54, Name = "ресторан-бар \"Коптильня\"", },
            new Place() { Id = 55, Name = "Ресторан «Форт»", },
            new Place() { Id = 56, Name = "Рок-бар \"DIESEL\"", },
            new Place() { Id = 57, Name = "Руки ВВерх! Бар", },
            new Place() { Id = 58, Name = "Сто Ручьёв", },
            new Place() { Id = 59, Name = "Театр драмы им. А. Кольцова", },
            new Place() { Id = 60, Name = "Театр Кот", },
            new Place() { Id = 61, Name = "Театр кукол им. В.А. Вольховского", },
            new Place() { Id = 62, Name = "Театр оперы и балета", },
            new Place() { Id = 63, Name = "Театр юного зрителя", },
            new Place() { Id = 64, Name = "«Тулиновъ Дом»", },
            new Place() { Id = 65, Name = "Усадьба \"Отрада\" (Калужская обл.)", },
            new Place() { Id = 66, Name = "Усадьба Скорняково-Архангельское", },
            new Place() { Id = 67, Name = "Хохольский центр развития культуры и туризма", },
            new Place() { Id = 68, Name = "Центральный Стадион Профсоюзов", },
            new Place() { Id = 69, Name = "Центр Галереи Чижова", },
            new Place() { Id = 70, Name = "ЛДС \"Юбилейный\"", },
            new Place() { Id = 71, Name = "ЯР Hotel&SPA", },
            new Place() { Id = 72, Name = "Музей-заповедник Дивногорье", },
            new Place() { Id = 73, Name = "Художественный музей им. Крамского", },
            new Place() { Id = 74, Name = "Стадион технических видов спорта Воронеж-Ринг", },
            new Place() { Id = 75, Name = "Портал на Фридриха Энгельса", },
            new Place() { Id = 76, Name = "Quest Brothers на Московском", },
            new Place() { Id = 77, Name = "Quest Brothers на Республиканской" },
            new Place() { Id = 78, Name = "Воронежская Филармония", ParentId = 18 },
            new Place() { Id = 79, Name = "Воронежская филармония", ParentId = 18 },
            new Place() { Id = 80, Name = "Aura Arena Hall", ParentId = 35 },
            new Place() { Id = 81, Name = "«Palazzo»" , ParentId = 4 },
            new Place() { Id = 82, Name = "Palazzo (ex. Arena Hall)", ParentId = 4 },
            new Place() { Id = 83, Name = "Дворец Культуры Железнодорожников", ParentId = 28 },
            new Place() { Id = 84, Name = "ДК железнодорожников", ParentId = 28 },
            new Place() { Id = 85, Name = "Сто ручьёв", ParentId = 58 },
            new Place() { Id = 86, Name = "Форт", ParentId = 55 },
            new Place() { Id = 87, Name = "Театр «Кот»", ParentId = 60 },
            new Place() { Id = 88, Name = "Театр Оперы и Балета", ParentId = 62 },
            new Place() { Id = 89, Name = "Воронежский государственный академический театр драмы им. А. Кольцова", ParentId = 59 },
            new Place() { Id = 90, Name = "Воронежский Дом актёра им. Л. Кравцовой", ParentId = 29 },
            new Place() { Id = 91, Name = "Театр «Дом Актера»", ParentId = 29 },
            new Place() { Id = 92, Name = "Дом Актера", ParentId = 29 },
            new Place() { Id = 93, Name = "МТС Live Холл (ex. Event Hall)", ParentId = 41 },
            new Place() { Id = 94, Name = "Музей- заповедник Дивногорье мастер классы", ParentId = 72 },
            new Place() { Id = 95, Name = "Музей- заповедник Дивногорье", ParentId = 72 },
            new Place() { Id = 96, Name = "Воронежский государственный театр юного зрителя", ParentId = 63 },
            new Place() { Id = 97, Name = "Воронежский Камерный театр", ParentId = 20 },
            new Place() { Id = 98, Name = "Пространство \"Винзавод\"", ParentId = 52 },
            new Place() { Id = 99, Name = "Железнодорожный вокзал", ParentId = 30 },
            new Place() { Id = 100, Name = "МБУК «Боринский ЦКРиД»" },
            new Place() { Id = 101, Name = "Pinta Haus" },
            new Place() { Id = 102, Name = "«На крыше под куполом» JAZZPARTY", ParentId = 44 },
            new Place() { Id = 103, Name = "Площадка 1900", ParentId = 51 },
            new Place() { Id = 104, Name = "Diesel", ParentId = 56 },
    };
}
