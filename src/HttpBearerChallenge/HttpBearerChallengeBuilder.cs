using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;

namespace Ragnar.HttpBearerChallenge
{
    /// <summary>
    /// Allows building HTTP bearer challenges.
    /// </summary>
    /// <remarks>
    /// For more information on what is an HTTP bearer challenge see: https://tools.ietf.org/html/rfc6750.
    /// The WWW-Authenticate header is used to return the challenge to the client.
    /// </remarks>
    public static partial class HttpBearerChallengeBuilder
    {
        #region Public Methods

        /// <summary>
        /// Builds a WWW-Authenticate header containing the HTTP bearer challenge that should be returned to the client.
        /// </summary>
        /// <param name="context">The bearer challenge context. This is the argument of the <see cref="JwtBearerEvents.Challenge(JwtBearerChallengeContext)"/> event.</param>
        /// <param name="scope">The scope that should be added in the header. It should be a space-delimited of case-sensitive scope values.</param>
        /// <example>
        /// This method should be called from the <see cref="JwtBearerEvents.Challenge(JwtBearerChallengeContext)"/> event like in the following example:
        /// <code>
        /// <![CDATA[
        /// // Add the JWT bearer middleware (in the REST API host)
        /// 
        /// authentication.AddJwtBearer(
        ///     (options) =>
        ///     {
        ///         (...)
        ///         options.Events = new JwtBearerEvents()
        ///         {
        ///             OnChallenge = HandleChallenge
        ///         };
        ///     });
        ///     
        /// // Handle the event
        /// 
        /// private static Task HandleChallenge(JwtBearerChallengeContext context)
        /// {
        ///     HttpBearerChallengeBuilder.BuildHeader(context, "MyScope");
        ///     return Task.CompletedTask;
        /// }
        /// ]]>
        /// </code>
        /// Alternatively, <see cref="HttpBearerChallengeEvents"/> can be used instead:
        /// <code>
        /// <![CDATA[
        /// // Add the JWT bearer middleware (in the REST API host)
        /// 
        /// authentication.AddJwtBearer(
        ///     (options) =>
        ///     {
        ///         (...)
        ///         options.Events = new HttpBearerChallengeEvents("MyScope");
        ///     });
        /// ]]>
        /// </code>
        /// </example>
        public static void BuildHeader(JwtBearerChallengeContext context, string scope)
        {
            if (context != null && context.Options != null)
            {
                // If there is a failure or an error in the context, then do not create the challenge

                if (context.AuthenticateFailure == null && string.IsNullOrEmpty(context.Error) && string.IsNullOrEmpty(context.ErrorDescription))
                {
                    // Create the WWW-Authenticate header with the challenge

                    CreateChallengeHeader(context, scope);
                }
            }
        }

        #endregion

        #region Private Methods

        private static void CreateChallengeHeader(JwtBearerChallengeContext context, string scope)
        {
            string value = BuildChallengeHeaderValue(context, scope);

            context.Response.Headers.TryAdd("WWW-Authenticate", value);

            context.Response.StatusCode = 401;

            context.HandleResponse();
        }

        private static string BuildChallengeHeaderValue(JwtBearerChallengeContext context, string scope)
        {
            List<string> parts = new List<string>();

            // realm

            string requestUri = context.Request.GetEncodedUrl();
            if (!string.IsNullOrEmpty(requestUri))
            {
                parts.Add($"realm=\"{requestUri}\"");
            }

            // authorization_uri

            if (!string.IsNullOrEmpty(context.Options.Authority))
            {
                parts.Add($"authorization_uri=\"{context.Options.Authority}\"");
            }

            // audience

            if (!string.IsNullOrEmpty(context.Options.Audience))
            {
                parts.Add($"audience=\"{context.Options.Audience}\"");
            }

            // scope

            if (!string.IsNullOrEmpty(scope))
            {
                parts.Add($"scope=\"{scope}\"");
            }

            return context.Options.Challenge + " " + string.Join(", ", parts);
        }

        #endregion
    }
}
