﻿namespace RemTechCommon.Utils.GuidUtils;

public sealed class RandomGuidGenerator : IGuidGenerationStrategy
{
    public Guid Generate() => Guid.NewGuid();
}
