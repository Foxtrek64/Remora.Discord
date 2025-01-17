//
//  ResponderGroup.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.Gateway.Responders;

/// <summary>
/// Enumerates various responder groups. Responders registered within a group run in parallel, but are ordered among
/// the groups.
/// </summary>
[PublicAPI]
public enum ResponderGroup
{
    /// <summary>
    /// This responder runs before all other groups.
    /// </summary>
    Early,

    /// <summary>
    /// This responder runs when responders normally run.
    /// </summary>
    Normal,

    /// <summary>
    /// This responder runs after all other groups.
    /// </summary>
    Late
}
