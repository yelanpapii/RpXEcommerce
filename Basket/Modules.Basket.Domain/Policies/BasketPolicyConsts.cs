using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Basket.Domain.Policies;

public static class BasketPolicyConsts
{
	public const string ReadPolicy = "basket:read";
	public const string CreatePolicy = "basket:create";
	public const string UpdatePolicy = "basket:update";
	public const string DeletePolicy = "basket:delete";
}
