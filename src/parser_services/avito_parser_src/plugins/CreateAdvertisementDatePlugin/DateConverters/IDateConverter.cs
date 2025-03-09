using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public interface IDateConverter
{
    Result<DateTime> ConvertToDateTime();
}
