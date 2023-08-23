using Microsoft.Extensions.Options;
using Playbill.Common.Event;
using Playbill.Services.ExportEvents.Common.Interfaces;

namespace Playbill.Services.ExportEvents.ToHtml;

public class ExportToHtmlService : IExportEvents
{
    private readonly ExportToHtmlOptions _options;
    public ExportToHtmlService(IOptions<ExportToHtmlOptions> exportToHtmlOptions)
    {
        _options = exportToHtmlOptions.Value;
    }

    public async Task ExportAync(IList<Event> events)
    {

        var htmlHeader = """
            <!DOCTYPE html>
            <html>

            <head>
                <link rel="stylesheet" href="style.css">
            </head>

            <body>
                <div class="events">
            """;
        var htmlFooter = """
                </div>

            </body>
            </html>
            """;
        var html = htmlHeader;
        foreach (var @event in events)
        {
            try
            {
                var buttons = "";
                @event.Links.ForEach(link => buttons += $"""
                <a href="{link.Path}">
                    <button>{link.BillboardType}</button>
                </a>
                """);

                var dateMask = @event.Date!.Value.Hour != 0 ? "dd MMMM HH:mm (dddd)" : "dd MMMM (dddd)";

                html += $"""
                        <div class="event event-{@event.Type.ToString().ToLower()}">
                            <div class="img-wrapper">
                                <img src ="{@event.ImagePath}">
                            </div>
                            <div class="info">
                                <h2>{@event.Title}</h2>
                                <p><b>{@event.Date.Value.ToString(dateMask)}</b></p>
                                <span>{@event.Place}</span>
                            </div>
                            <div class="buttons">
                {buttons}
                            </div>
                        </div>
                """;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"Fail create html event: {exception.Message}");
            }
        }
        html += htmlFooter;

        if (!Directory.Exists(_options.OutputFolder))
        {
            Directory.CreateDirectory(_options.OutputFolder);
        }
        var path = Path.Combine(_options.OutputFolder, _options.OutputHtmlFileName);

        using (var outputFile = File.CreateText(path))
        {
            await outputFile.WriteLineAsync(html);
        }

        File.Copy(_options.CssFilePath, 
            Path.Combine(_options.OutputFolder, 
            Path.GetFileName(_options.CssFilePath)), 
            true);
    }
}
