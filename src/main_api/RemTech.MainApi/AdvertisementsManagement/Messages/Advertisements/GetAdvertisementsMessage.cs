using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.AdvertisementsManagement.Models;

namespace RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;

public sealed record GetAdvertisementsMessage(AdvertisementsQuery Query) : IContract;

public sealed record TransportAdvertisementDaoResponse(
    TransportAdvertisement[] Advertisements,
    long Count
);
