using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Helpers;

public static class MarkExcludeSettingsHelper
{
    public static void SetExclude<T>(IEnumerable<T> excludeCollection, List<Setting> collection)
    {
        excludeCollection.ToList().ForEach(excludeEntity =>
        {
            var entity = collection.FirstOrDefault(entity => entity.Id == Convert.ToInt32(excludeEntity));
            if (entity is not null)
            {
                entity.Exclude = true;
            }
        });
    }
}
