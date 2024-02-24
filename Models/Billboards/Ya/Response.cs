using Models.Billboards.Common.Interfaces;
using Models.Events;

namespace Models.Billboards.Ya;

public class Response : IConvertToEvent<string>
{
    public PaginationOptions Paging { get; set; }
    public List<Datum> Data { get; set; }

    public IList<Event> ConvertToEvents(BaseConvertToEventSetting convertToEventSetting)
    {
        var setting = convertToEventSetting as ConvertToEventSetting;
        if (!Data.Any()) return new List<Event>();

        var resultEvent = new List<Event>();
        foreach (var item in Data)
        {
            try
            {
                var eventType = setting.EventType;
                var date = new List<DateTime>();
                if (item.ScheduleInfo.Regularity?.SingleShowtime != null)
                {
                    date.Add(item.ScheduleInfo.Regularity.SingleShowtime.Value);
                }
                else if (item.ScheduleInfo.Dates?.Any() ?? false)// && (item.ScheduleInfo.Regularity?.Daily?.Any() ?? false)
                {
                    item.ScheduleInfo.Dates.ForEach(scheduleInfoDate =>
                    {
                        var eventDate = DateOnly.Parse(scheduleInfoDate);
                        if (item.ScheduleInfo.Regularity.Daily.Any())
                        {
                            item.ScheduleInfo.Regularity.Daily.ForEach(time =>
                            {
                                var eventTime = TimeOnly.Parse(time);
                                date.Add(new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, eventTime.Hour, eventTime.Minute, 0));
                            });
                        } else
                        {
                            date.Add(new DateTime(eventDate.Year, eventDate.Month, eventDate.Day));
                        }
                    });      
                }
                else
                {
                    date.Add(DateTime.Parse(item.ScheduleInfo.DateStarted));
                }
                var title = item.@Event.Title;
                var imagePath = item.@Event.PromoImage2?.PreviewL?.Url
                    ?? item.@Event.PromoImage2?.PreviewM?.Url
                    ?? item.@Event.PromoImage2?.PreviewS?.Url
                    ?? item.@Event.PromoImage2?.PreviewXS?.Url
                    ?? item.@Event.PromoImage2?.FeaturedDesktop?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverL2x?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverL?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverM?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverM2x?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverS?.Url
                    ?? item.@Event.Image?.Sizes?.EventCoverXS?.Url
                    ?? null;
                var place = item.ScheduleInfo.PlacePreview;
                var link = setting.BaseLinkUrl + item.@Event.Url;

                var @event = new Event()
                {
                    Billboard = BillboardTypes.Ya,
                    Type = eventType,
                    Dates = date,
                    Title = title,
                    ImagePath = imagePath,
                    Place = place,
                    Links = new List<EventLink>()
                    {
                        new EventLink()
                        {
                            BillboardType = BillboardTypes.Ya,
                            Path = link
                        }
                    }
                };
                resultEvent.Add(@event);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Fail convert (Ya, {setting.EventType}): {item.@Event?.Title ?? "?"} Message : {exception.Message}");
            }
        }
        return resultEvent;
    }

    public class City
    {
        public string id { get; set; }
        public string name { get; set; }
        public int geoid { get; set; }
        public string timezone { get; set; }
    }

    public class Coordinates
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class DateGroup
    {
        public string title { get; set; }
        public string date { get; set; }
        public int period { get; set; }
        public bool hasTickets { get; set; }
        public bool hasDiscounts { get; set; }
    }

    public class Datum
    {
        public object distance { get; set; }
        public bool isPersonal { get; set; }
        public int commentsCount { get; set; }
        public EventYa @Event { get; set; }
        public ScheduleInfo ScheduleInfo { get; set; }
    }

    public class EventYa
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool Permanent { get; set; }
        public List<SystemTag> SystemTags { get; set; }
        public string Title { get; set; }
        public object OriginalTitle { get; set; }
        public object DateReleased { get; set; }
        public string Argument { get; set; }
        public object PromoArgument { get; set; }
        public string ContentRating { get; set; }
        public object Kinopoisk { get; set; }
        public bool IsFavorite { get; set; }
        public Type Type { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Ticket> Tickets { get; set; }
        public object Poster { get; set; }
        public PromoImage2 PromoImage2 { get; set; }
        public PromoVideo2 PromoVideo2 { get; set; }
        public Image Image { get; set; }
    }

    public class EventCover
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverL
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverL2x
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverM
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverM2x
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class EventCoverXS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class Featured
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class FeaturedDesktop
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
        public Retina Retina { get; set; }
    }

    public class FeaturedSelection
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class HeadingPrimaryS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class Image
    {
        public object SubType { get; set; }
        public string BgColor { get; set; }
        public string BaseColor { get; set; }
        public Source Source { get; set; }
        public Sizes Sizes { get; set; }
    }

    public class Logo
    {
        public string bgColor { get; set; }
        public Microdata microdata { get; set; }
        public Place place { get; set; }
        public PlaceCover placeCover { get; set; }
        public PlaceCoverXS placeCoverXS { get; set; }
        public PlaceCoverM placeCoverM { get; set; }
        public TouchPlace touchPlace { get; set; }
        public TouchPlaceCard touchPlaceCard { get; set; }
        public TouchPlaceCover touchPlaceCover { get; set; }
        public TouchConcertPlace touchConcertPlace { get; set; }
    }

    public class Microdata
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class NameCases
    {
        public string nameAcc { get; set; }
        public string nameGen { get; set; }
    }

    public class OneOfPlaces
    {
        public string id { get; set; }
        public string Url { get; set; }
        public Type type { get; set; }
        public List<Tag> tags { get; set; }
        public Logo logo { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public List<SystemTag> systemTags { get; set; }
        public City city { get; set; }
        public Coordinates coordinates { get; set; }
        public string bgColor { get; set; }
        public string logoColor { get; set; }
        public object distance { get; set; }
        public object promoImage2FeaturedDesktop { get; set; }
        public object promoImage2FeaturedBannerDesktop { get; set; }
        public object promoVideo2FeaturedDesktop { get; set; }
        public List<object> metro { get; set; }
        public bool isFavorite { get; set; }
    }

    public class OnlyPlace
    {
        public string id { get; set; }
        public string Url { get; set; }
        public Type type { get; set; }
        public List<Tag> tags { get; set; }
        public Logo logo { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public List<SystemTag> systemTags { get; set; }
        public City city { get; set; }
        public Coordinates coordinates { get; set; }
        public string bgColor { get; set; }
        public string logoColor { get; set; }
        public object distance { get; set; }
        public object promoImage2FeaturedDesktop { get; set; }
        public object promoImage2FeaturedBannerDesktop { get; set; }
        public object promoVideo2FeaturedDesktop { get; set; }
        public List<object> metro { get; set; }
        public bool isFavorite { get; set; }
    }

    public class Overall
    {
        public int value { get; set; }
        public int count { get; set; }
    }

    public class PaginationOptions
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class Place
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class PlaceCover
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class PlaceCoverM
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class PlaceCoverXS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class Preview
    {
        public string type { get; set; }
        public string text { get; set; }
        public object startDate { get; set; }
        public object regularity { get; set; }
        public SingleDate singleDate { get; set; }
    }

    public class PreviewL
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
        public Retina Retina { get; set; }
    }

    public class PreviewM
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
        public Retina retina { get; set; }
    }

    public class PreviewS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
        public Retina retina { get; set; }
    }

    public class PreviewXS
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
        public Retina retina { get; set; }
    }

    public class Price
    {
        public string currency { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }

    public class Price2
    {
        public string currency { get; set; }
        public int value { get; set; }
    }

    public class PromoImage2
    {
        public FeaturedDesktop FeaturedDesktop { get; set; }
        public PreviewL PreviewL { get; set; }
        public PreviewM PreviewM { get; set; }
        public PreviewS PreviewS { get; set; }
        public PreviewXS PreviewXS { get; set; }
    }

    public class PromoVideo2
    {
    }

    public class Regularity
    {
        public bool IsRegular { get; set; }
        public DateTime? SingleShowtime { get; set; }
        public List<string> Daily { get; set; }
        public List<object> Weekly { get; set; }
    }

    public class Retina
    {
        //[JsonProperty("1x")]
        //public string _1x { get; set; }

        //[JsonProperty("2x")]
        //public string _2x { get; set; }
    }

    public class ScheduleInfo
    {
        public object CollapsedText { get; set; }
        public string DateEnd { get; set; }
        public List<DateGroup> DateGroups { get; set; }
        public object DateReleased { get; set; }
        public List<string> Dates { get; set; }
        public string DateStarted { get; set; }
        public bool MultiSession { get; set; }
        public OneOfPlaces OneOfPlaces { get; set; }
        public OnlyPlace OnlyPlace { get; set; }
        public bool Permanent { get; set; }
        public string PlacePreview { get; set; }
        public int PlacesTotal { get; set; }
        public Preview Preview { get; set; }
        public Regularity Regularity { get; set; }
        public object TagsPreview { get; set; }
        public List<Price> Prices { get; set; }
        public bool PushkinCardAllowed { get; set; }
    }

    public class SingleDate
    {
        public string day { get; set; }
        public string month { get; set; }
    }

    public class Sizes
    {
        public EventCover EventCover { get; set; }
        public EventCoverXS EventCoverXS { get; set; }
        public EventCoverS EventCoverS { get; set; }
        public EventCoverL EventCoverL { get; set; }
        public EventCoverL2x EventCoverL2x { get; set; }
        public EventCoverM EventCoverM { get; set; }
        public EventCoverM2x EventCoverM2x { get; set; }
        public Featured Featured { get; set; }
        public FeaturedSelection FeaturedSelection { get; set; }
        public HeadingPrimaryS HeadingPrimaryS { get; set; }
        public Microdata Microdata { get; set; }
        public Suggest Suggest { get; set; }
    }

    public class Source
    {
        public object id { get; set; }
        public string Url { get; set; }
        public string title { get; set; }
    }

    public class Suggest
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class SystemTag
    {
        public string code { get; set; }
    }

    public class Tag
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public string namePlural { get; set; }
        public object plural { get; set; }
        public string rubricPlacesUrl { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public NameCases nameCases { get; set; }
        public string rubricUrl { get; set; }
        public string nameAcc { get; set; }
        public object nameGen { get; set; }
        public object nameAdj { get; set; }
    }

    public class Ticket
    {
        public string id { get; set; }
        public Price price { get; set; }
        public object discount { get; set; }
        public string saleStatus { get; set; }
        public bool hasSpecificLoyalty { get; set; }
        public bool hasSpecificPlusWalletPercent { get; set; }
        public int plusWalletPercent { get; set; }
        public List<object> discountPercents { get; set; }
    }

    public class TouchConcertPlace
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class TouchPlace
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class TouchPlaceCard
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class TouchPlaceCover
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Precision { get; set; }
    }

    public class Type
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public string namePlural { get; set; }
        public object plural { get; set; }
        public string rubricPlacesUrl { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public NameCases nameCases { get; set; }
        public string rubricUrl { get; set; }
        public string nameAcc { get; set; }
        public object nameGen { get; set; }
        public object nameAdj { get; set; }
    }

    public class UserRating
    {
        public Overall overall { get; set; }
    }

}
