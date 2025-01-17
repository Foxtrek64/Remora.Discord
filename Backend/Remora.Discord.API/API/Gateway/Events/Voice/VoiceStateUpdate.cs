//
//  VoiceStateUpdate.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Gateway.Events;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IVoiceStateUpdate" />
[PublicAPI]
public record VoiceStateUpdate
(
    Optional<Snowflake> GuildID,
    Snowflake? ChannelID,
    Snowflake UserID,
    Optional<IGuildMember> Member,
    string SessionID,
    bool IsDeafened,
    bool IsMuted,
    bool IsSelfDeafened,
    bool IsSelfMuted,
    Optional<bool> IsStreaming,
    bool IsVideoEnabled,
    bool IsSuppressed,
    DateTimeOffset? RequestToSpeakTimestamp
) : VoiceState
(
    GuildID,
    ChannelID,
    UserID,
    Member,
    SessionID,
    IsDeafened,
    IsMuted,
    IsSelfDeafened,
    IsSelfMuted,
    IsStreaming,
    IsVideoEnabled,
    IsSuppressed,
    RequestToSpeakTimestamp
), IVoiceStateUpdate;
