﻿using RunOtp.Domain.WebConfigurationAggregate;

namespace RunOtp.WebApi.UseCase.WebConfigurations;

public record WebConfigurationDto(
    Guid Id,
    string ApiSecret,
    string Url,
    string WebName,
    string Endpoint,
    WebStatus Status,
    bool Selected,
    DateTimeOffset CreatedDate,
    DateTimeOffset LastUpdatedDate
);