//
//  DiscordRateLimitPolicy.cs
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
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Remora.Discord.Rest.API;

namespace Remora.Discord.Rest.Polly;

/// <summary>
/// Represents a Discord rate limiting policy.
/// </summary>
internal class DiscordRateLimitPolicy : AsyncPolicy<HttpResponseMessage>
{
    private readonly ConcurrentDictionary<string, RateLimitBucket> _rateLimitBuckets;
    private RateLimitBucket _globalRateLimitBucket;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRateLimitPolicy"/> class.
    /// </summary>
    private DiscordRateLimitPolicy()
    {
        _globalRateLimitBucket = new RateLimitBucket
        (
            50,
            50,
            DateTimeOffset.UtcNow + TimeSpan.FromSeconds(1),
            "global",
            true
        );

        _rateLimitBuckets = new ConcurrentDictionary<string, RateLimitBucket>();
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> ImplementationAsync
    (
        Func<Context, CancellationToken, Task<HttpResponseMessage>> action,
        Context context,
        CancellationToken cancellationToken,
        bool continueOnCapturedContext
    )
    {
        if (!context.TryGetValue("endpoint", out var rawEndpoint) || rawEndpoint is not string endpoint)
        {
            throw new InvalidOperationException("No endpoint set.");
        }

        var now = DateTimeOffset.UtcNow;

        // Determine whether this request is exempt from global rate limits
        var isExemptFromGlobalRateLimits = false;
        if (context.TryGetValue("exempt-from-global-rate-limits", out var rawExempt) && rawExempt is bool isExempt)
        {
            isExemptFromGlobalRateLimits = isExempt;
        }

        // First, take a token from the global limits
        if (!isExemptFromGlobalRateLimits)
        {
            // Check if we need to reset the global limits
            if (_globalRateLimitBucket.ResetsAt < now)
            {
                await _globalRateLimitBucket.ResetAsync(now + TimeSpan.FromSeconds(1));
            }

            if (!await _globalRateLimitBucket.TryTakeAsync())
            {
                var rateLimitedResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

                var delay = _globalRateLimitBucket.ResetsAt - now;
                rateLimitedResponse.Headers.RetryAfter = new RetryConditionHeaderValue(delay);

                return rateLimitedResponse;
            }
        }

        // Then, try to take one from the local bucket
        if (_rateLimitBuckets.TryGetValue(endpoint, out var rateLimitBucket))
        {
            // We don't reset route-specific rate limits ourselves; that's the responsibility of the returned headers
            // from Discord
            if (!await rateLimitBucket.TryTakeAsync())
            {
                var rateLimitedResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

                var delay = rateLimitBucket.ResetsAt - now;
                rateLimitedResponse.Headers.RetryAfter = new RetryConditionHeaderValue(delay);

                return rateLimitedResponse;
            }
        }

        // The request can proceed without hitting rate limits, and we've taken a token
        var requestAction = action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);

        var response = await requestAction;
        if (!RateLimitBucket.TryParse(response.Headers, out var newLimits))
        {
            return response;
        }

        if (newLimits.IsGlobal)
        {
            if (_globalRateLimitBucket.ResetsAt < newLimits.ResetsAt)
            {
                _globalRateLimitBucket = newLimits;
            }

            return response;
        }

        _rateLimitBuckets.AddOrUpdate
        (
            endpoint,
            newLimits,
            (_, old) => old.ResetsAt < newLimits.ResetsAt ? newLimits : old
        );

        return response;
    }

    /// <summary>
    /// Creates a new instance of the policy.
    /// </summary>
    /// <returns>The policy.</returns>
    public static DiscordRateLimitPolicy Create() => new();
}
