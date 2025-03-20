using Rabbit.RPC.Client.Abstractions;

namespace SharedParsersLibrary.Sinking;

public sealed record SinkAdvertisement(SinkingAdvertisement Advertisement) : IContract;
