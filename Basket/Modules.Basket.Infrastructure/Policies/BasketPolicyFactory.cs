using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Modules.Basket.Domain.Policies;
using Modules.Common.Infrastructure.Policies;

namespace Modules.Basket.Infrastructure.Policies;

internal sealed class BasketPolicyFactory : IPolicyFactory
{
	public string ModuleName => "Basket";

	public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
	{
		return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
		{
			[BasketPolicyConsts.ReadPolicy] = policy => policy.RequireClaim(BasketPolicyConsts.ReadPolicy),
			[BasketPolicyConsts.CreatePolicy] = policy => policy.RequireClaim(BasketPolicyConsts.CreatePolicy),
			[BasketPolicyConsts.UpdatePolicy] = policy => policy.RequireClaim(BasketPolicyConsts.UpdatePolicy),
			[BasketPolicyConsts.DeletePolicy] = policy => policy.RequireClaim(BasketPolicyConsts.DeletePolicy)
		};
	}
}
