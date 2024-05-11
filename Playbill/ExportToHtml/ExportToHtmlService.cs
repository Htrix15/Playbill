using Microsoft.Extensions.Options;
using Models.Events;
using System.Diagnostics;

namespace ConsoleApp.ExportToHtml;

public class ExportToHtmlService(IOptions<ExportToHtmlOptions> exportToHtmlOptions)
{
    private readonly ExportToHtmlOptions _options = exportToHtmlOptions.Value;

    private string AddEventToPage(Event @event)
    {
        var eventBlock = "";
        try
        {
            var buttons = "";
            @event.Links.ForEach(link => buttons += $"""
                <a href="{link.Path}">
                    <button>{link.BillboardType}</button>
                </a>
                """);

            string date = "";

            if (!@event.SubstandardByDate)
            {
                var dateMask = @event.Date!.Value.Hour != 0 ? "dd MMMM HH:mm (dddd)" : "dd MMMM (dddd)";
                var times = @event.Sessions?.Any() ?? false
                    ? $"({string.Join(", ", @event.Sessions.Select(session => session.ToString("HH:mm")))})"
                    : "";

                date = $"<b>{@event.Date.Value.ToString(dateMask)}</b> {times}";
            }

            eventBlock += $"""
                        <div class="event event-{@event.Type.ToString().ToLower()}">
                            <div class="img-wrapper">
                                <img src ="{@event.ImagePath}">
                            </div>
                            <div class="info">
                                <h2>{@event.Title}</h2>
                                <p>{date}</p>
                                <span>{@event.Place}</span>
                            </div>
                            <div class="buttons">
                {buttons}
                            </div>
                        </div>
                """;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail create html event: {exception.Message}");
        }

        return eventBlock;
    }

    public async Task ExportAync(IList<Event> events)
    {
        var style = await File.ReadAllTextAsync(_options.CssFilePath);
        var htmlHeader = $"""
            <!DOCTYPE html>
            <html>

            <head>
                 <style>
                    {style}
                </style>
            </head>

            <body>
            """;
        var htmlFooter = """
            </body>
            </html>
            """;
        var html = htmlHeader;

        var eventBlocks = "<div class=\"events\">";

        foreach (var @event in events.Where(e => !e.Substandard))
        {
            eventBlocks += AddEventToPage(@event);
        }
        eventBlocks += "</div>";

        var substandardEvents = events.Where(e => e.Substandard).ToList();
        if (substandardEvents.Any())
        {
            eventBlocks += """
                <hr>
                <p>События которые не получилось распознать полностью</p>
                <div class="events">
                """;
            foreach (var @event in substandardEvents)
            {
                eventBlocks += AddEventToPage(@event);
            }
            eventBlocks += "</div>";
        }

        html += eventBlocks + htmlFooter;

        if (!Directory.Exists(_options.OutputFolder))
        {
            Directory.CreateDirectory(_options.OutputFolder);
        }
        var path = Path.Combine(_options.OutputFolder, _options.OutputHtmlFileName);

        using (var outputFile = File.CreateText(path))
        {
            await outputFile.WriteLineAsync(html);
        }

        if (_options.OpenFileAfterCreate)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = path
                }
            };
            process.Start();
        }
    }
}
