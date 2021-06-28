using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ragnar.HttpBearerChallenge
{
    /// <summary>
    /// Specifies events that <see cref="JwtBearerHandler"/> invokes during the
    /// authentication process. This class automatically builds a HTTP Bearer challenge,
    /// using <see cref="HttpBearerChallengeBuilder"/> in the <see cref="JwtBearerEvents.OnChallenge"/>
    /// event.
    /// </summary>
    public partial class HttpBearerChallengeEvents : JwtBearerEvents
    {
        #region Private Properties

        private string DefaultScope
        {
            get;
        }

        #endregion

        #region Constructors

        // <summary>
        /// Initializes a new instance of the <see cref="HttpBearerChallengeEvents"/> class.
        /// </summary>
        public HttpBearerChallengeEvents()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpBearerChallengeEvents"/> class.
        /// </summary>
        /// <param name="scope">The scope that should be included in the challenge.</param>
        public HttpBearerChallengeEvents(string defaultScope)
        {
            if (string.IsNullOrWhiteSpace(defaultScope))
            {
                throw new ArgumentNullException(nameof(defaultScope));
            }

            this.DefaultScope = defaultScope;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            ILogger<HttpBearerChallengeEvents> logger = context?.HttpContext?.RequestServices?.GetService<ILogger<HttpBearerChallengeEvents>>();
            logger?.LogDebug("Building Bearer challenge header...");

            string scope = this.DefaultScope;

            if (context.HttpContext.Items.ContainsKey("_ScopeAuthorizationRequirement.Scope"))
            {
                scope = context.HttpContext.Items["_ScopeAuthorizationRequirement.Scope"] as string;
            }

            HttpBearerChallengeBuilder.BuildHeader(context, scope);

            return base.Challenge(context);
        }

        #endregion
    }
}
