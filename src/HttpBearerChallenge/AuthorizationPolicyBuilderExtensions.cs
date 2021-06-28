using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Ragnar.HttpBearerChallenge
{
    /// <summary>
    /// Provides extension methods for the <see cref="AuthorizationPolicyBuilder"/> type.
    /// </summary>
    public static partial class AuthorizationPolicyBuilderExtensions
    {
        #region Public Methods

        /// <summary>
        /// Adds a <see cref="ScopeAuthorizationRequirement" /> to the current instance which requires
        /// that the current user has the specified scope.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>
        /// The <see cref="AuthorizationPolicyBuilder" /> instance.
        /// </returns>
        /// <remarks>
        /// If the scope is not present, the requirement will add it to the <see cref="HttpContext"/> items
        /// so that the WWW-Authenticate header can be built correctly.
        /// </remarks>
        public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrWhiteSpace(scope))
            {
                throw new ArgumentNullException(nameof(scope));
            }

            builder.Requirements.Add(
                new ScopeAuthorizationRequirement(
                    scope));

            return builder;
        }

        #endregion
    }
}
