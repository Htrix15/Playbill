using Playbill.Common.Event;

namespace Playbill.Services.ExportEvents.Common.Interfaces;

public interface IExportEvents
{
    Task ExportAync(IList<Event> events);
}
