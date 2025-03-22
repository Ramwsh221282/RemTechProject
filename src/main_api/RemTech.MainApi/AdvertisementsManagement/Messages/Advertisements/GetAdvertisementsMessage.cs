using Rabbit.RPC.Client.Abstractions;

namespace RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;

public sealed record GetAdvertisementsMessage(AdvertisementsQuery Query) : IContract;
