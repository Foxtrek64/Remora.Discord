//
//  ITeam.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a developer team on Discord.
/// </summary>
[PublicAPI]
public interface ITeam
{
    /// <summary>
    /// Gets the team's icon.
    /// </summary>
    IImageHash? Icon { get; }

    /// <summary>
    /// Gets the ID of the team.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the team members.
    /// </summary>
    IReadOnlyList<ITeamMember> Members { get; }

    /// <summary>
    /// Gets the name of the team.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the user ID of the current team owner.
    /// </summary>
    Snowflake OwnerUserID { get; }
}
