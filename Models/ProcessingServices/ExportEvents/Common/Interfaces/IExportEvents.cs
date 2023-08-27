using Models.Events;

namespace Models.ProcessingServices.ExportEvents.Common.Interfaces;

public interface IExportEvents
{
    Task ExportAync(IList<Event> events);
}
