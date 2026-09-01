using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Modules.Catalog.Domain.Policies;
using Modules.Common.Infrastructure.Policies;

namespace Modules.Catalog.Infrastructure.Policies;

internal sealed class CatalogPolicyFactory : IPolicyFactory
{
	public string ModuleName => "Catalog";

	public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
	{
		return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
		{
			[CatalogPolicyConsts.ReadPolicy] = policy => policy.RequireClaim(CatalogPolicyConsts.ReadPolicy),
			[CatalogPolicyConsts.CreatePolicy] = policy => policy.RequireClaim(CatalogPolicyConsts.CreatePolicy),
			[CatalogPolicyConsts.UpdatePolicy] = policy => policy.RequireClaim(CatalogPolicyConsts.UpdatePolicy),
			[CatalogPolicyConsts.DeletePolicy] = policy => policy.RequireClaim(CatalogPolicyConsts.DeletePolicy)
		};
	}
}
