using System.Globalization;
using System.Xml.Serialization;

namespace Playbill.Services.EventDateIntervals.Common;


[XmlRoot(ElementName = "holiday")]
public class HolidayXmlRespose
{

    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; }
}

[XmlRoot(ElementName = "holidays")]
public class Holidays
{

    [XmlElement(ElementName = "holiday")]
    public List<HolidayXmlRespose> Holiday { get; set; }
}

[XmlRoot(ElementName = "day")]
public class Day
{

    [XmlAttribute(AttributeName = "d")]
    public string D { get; set; }

    [XmlAttribute(AttributeName = "t")]
    public int T { get; set; }

    [XmlAttribute(AttributeName = "h")]
    public int H { get; set; }

    [XmlAttribute(AttributeName = "f")]
    public string F { get; set; }
}

[XmlRoot(ElementName = "days")]
public class Days
{

    [XmlElement(ElementName = "day")]
    public List<Day> Day { get; set; }
}

[XmlRoot(ElementName = "calendar")]
public class HolidaysResponse
{

    [XmlElement(ElementName = "holidays")]
    public Holidays Holidays { get; set; }

    [XmlElement(ElementName = "days")]
    public Days Days { get; set; }

    [XmlAttribute(AttributeName = "year")]
    public int Year { get; set; }

    [XmlAttribute(AttributeName = "lang")]
    public string Lang { get; set; }

    [XmlAttribute(AttributeName = "date")]
    public string Date { get; set; }

    [XmlAttribute(AttributeName = "country")]
    public string Country { get; set; }

    public List<DateOnly> OnlyHolidaysDays() => Days.Day
            .Where(day => day.T == 1) //http://xmlcalendar.ru/
            .Select(day => DateOnly.ParseExact(day.D, "MM.dd", CultureInfo.CurrentCulture))
            .ToList();
}