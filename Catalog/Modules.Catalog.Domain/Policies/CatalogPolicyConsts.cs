using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Policies;

public static class CatalogPolicyConsts
{
	public const string ReadPolicy = "catalog:read";
	public const string CreatePolicy = "catalog:create";
	public const string UpdatePolicy = "catalog:update";
	public const string DeletePolicy = "catalog:delete";
	
}
