using System.Reflection;

namespace Modules.Catalog.Infrastructure;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
