using System.Reflection;

namespace Modules.Catalog.PublicApi;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
