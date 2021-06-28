using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Ragnar.HttpBearerChallenge
{
    /// <summary>
    /// Implements an <see cref="IAuthorizationHandler"/> and <see cref="IAuthorizationRequirement"/>
    /// which requires the specified scope.
    /// </summary>
    /// <remarks>
    /// This requirement will add it to the <see cref="HttpContext"/> items
    /// so that the WWW-Authenticate header can be built correctly.
    /// </remarks>
    public partial class ScopeAuthorizationRequirement : AuthorizationHandler<ScopeAuthorizationRequirement>, IAuthorizationRequirement
    {
        #region Public Properties

        /// <summary>
        /// Gets the scope that must be present.
        /// </summary>
        public string Scope
        {
            get;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAuthorizationRequirement"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public ScopeAuthorizationRequirement(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope))
            {
                throw new ArgumentNullException(nameof(scope));
            }

            this.Scope = scope;
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAuthorizationRequirement requirement)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            if (context.User != null)
            {
                if (context.User.Claims.Any(i => i.Type.Equals("scope", StringComparison.OrdinalIgnoreCase) && i.Value.Equals(this.Scope, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    if (context.Resource is HttpContext httpContext)
                    {
                        if (httpContext.Items.ContainsKey("_ScopeAuthorizationRequirement.Scope"))
                        {
                            string value = httpContext.Items["_ScopeAuthorizationRequirement.Scope"] as string;
                            httpContext.Items["_ScopeAuthorizationRequirement.Scope"] = string.Join(" ", value, this.Scope);
                        }
                        else
                        {
                            httpContext.Items.Add("_ScopeAuthorizationRequirement.Scope", this.Scope);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
