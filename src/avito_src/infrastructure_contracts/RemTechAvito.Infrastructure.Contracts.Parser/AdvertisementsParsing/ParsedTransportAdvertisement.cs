﻿using System.Text.RegularExpressions;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;

public sealed record ParsedTransportAdvertisement(
    string Url,
    string Id,
    string Title,
    ParsedTransportAdvertisementSellerInfo SellerInfo,
    string[] Characteristics,
    string Address,
    string Description,
    string Date,
    string[] PhotoLinks,
    ParsedTransportAdvertisementPriceInfo PriceInfo
);

public sealed record ParsedTransportAdvertisementSellerInfo(
    string Name,
    string Status,
    string Contacts
);

public sealed record ParsedTransportAdvertisementPriceInfo(
    string Value,
    string Currency,
    string Extra
);

public static class ParsedTransportAdvertisementExtensions
{
    public static Result<TransportAdvertisement> ToTransportAdvertisement(
        this ParsedTransportAdvertisement parsed
    )
    {
        var adId = AdvertisementID.Create(parsed.Id);
        if (adId.IsFailure)
            return adId.Error;

        var title = Title.Create(parsed.Title);
        if (title.IsFailure)
            return title.Error;

        var description = Description.Create(parsed.Description);
        if (description.IsFailure)
            return description.Error;

        var price = parsed.PriceInfo.ToValueObject();
        if (price.IsFailure)
            return price.Error;

        Result<OwnerInformation> ownerInfo = parsed.SellerInfo.ToValueObject();
        if (ownerInfo.IsFailure)
            return ownerInfo.Error;

        var address = Address.Create(parsed.Address);
        if (address.IsFailure)
            return address.Error;

        var ctx = new Characteristics(parsed.GetCharacteristicsList());

        var photos = new PhotoAttachments(parsed.GetPhotosList());

        var url = AdvertisementUrl.Create(parsed.Url);
        if (url.IsFailure)
            return url.Error;

        var advertisement = new TransportAdvertisement(
            adId,
            ctx,
            address,
            ownerInfo,
            photos,
            price,
            title,
            description,
            DateOnly.FromDateTime(DateTime.Now),
            url
        );

        return advertisement;
    }

    public static Result<Price> ToValueObject(this ParsedTransportAdvertisementPriceInfo price)
    {
        return Price.Create(price.Value, price.Currency, price.Extra);
    }

    public static Result<OwnerInformation> ToValueObject(
        this ParsedTransportAdvertisementSellerInfo info
    )
    {
        return OwnerInformation.Create(info.Name, info.Status);
    }

    public static List<Characteristic> GetCharacteristicsList(
        this ParsedTransportAdvertisement advertisement
    )
    {
        var ctxParsed = advertisement.Characteristics;
        List<Characteristic> ctxConverted = [];
        foreach (var element in ctxParsed)
            try
            {
                var splitted = element.Split(':');
                var name = splitted[0];
                var value = splitted[1];
                var ctx = Characteristic.Create(name, value);
                if (ctx.IsSuccess)
                    ctxConverted.Add(ctx.Value);
            }
            catch
            {
                // ignored
            }

        return ctxConverted;
    }

    public static List<Photo> GetPhotosList(this ParsedTransportAdvertisement advertisement)
    {
        List<Photo> photos = [];
        photos.AddRange(
            advertisement
                .PhotoLinks.Select(parsed => Photo.Create(parsed))
                .Where(photo => photo.IsSuccess)
                .Select(photo => (Photo)photo)
        );

        return photos;
    }

    public static bool IsFollowingAdditions(
        this ParsedTransportAdvertisement advertisement,
        IEnumerable<string> additions
    )
    {
        string[] attributes =
        [
            advertisement.Address,
            advertisement.Description,
            .. advertisement.Characteristics,
            advertisement.Title,
            advertisement.SellerInfo.Status,
            advertisement.SellerInfo.Name,
        ];

        return additions
            .Select(addition => $".*{Regex.Escape(addition)}.*")
            .Any(regExp => attributes.Any(a => Regex.IsMatch(a, regExp)));
    }
}
