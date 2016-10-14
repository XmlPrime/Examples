using System;
using System.Xml;

namespace XmlPrime.Samples.CompilingModules
{
	/// <summary>
	/// A simple example demonstrating use of the <see cref="LibrarySet"/> class.
	/// </summary>
	internal class Program
	{
		#region Private Static Methods

		/// <summary>
		/// Returns a library set containing a compiled module.
		/// </summary>
		/// <param name="resolver">The resolver.</param>
		/// <returns>A new <see cref="LibrarySet" /> instance.</returns>
		private static LibrarySet CompileModule(XmlResolver resolver)
		{
			var moduleSettings = new XQuerySettings {ModuleResolver = resolver};

			var librarySet = new LibrarySet();
			librarySet.Add(moduleSettings, "urn:module", "assembly:module.xqm");

			librarySet.Compile();
			return librarySet;
		}

		/// <summary>
		/// Compiles the query.
		/// </summary>
		/// <param name="librarySet">The library set.</param>
		/// <param name="resolver">The resolver.</param>
		/// <returns>A new <see cref="XQuery" /> instance.</returns>
		private static XQuery CompileQuery(LibrarySet librarySet, XmlResolver resolver)
		{
			var settings = new XQuerySettings {ModuleResolver = resolver, Libraries = librarySet, ContextItemType = XdmType.Node};

			using (var stream = typeof (Program).Assembly.GetManifestResourceStream("XmlPrime.Samples.CompilingModules.query.xq"))
			{
				return XQuery.Compile(stream, settings);
			}
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// The entry point to the sample application.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		public static void Main(string[] args)
		{
			// Create a resolver to resolve embedded resources.
			var resolver = new XmlManifestResolver(typeof (Program).Assembly);

			// Create a library set containing a compiled module.
			var librarySet = CompileModule(resolver);

			// Compile a query using the compiled module.
			var query = CompileQuery(librarySet, resolver);

			if (query != null)
			{
			    var dynamicContext = new DynamicContextSettings();
				dynamicContext.Parameters.Add(new XmlQualifiedName("external", "urn:module"),(double) 5);

				// Serialize the results of executing the query to the console.
				query.Serialize(Console.Out, dynamicContext);
			}
		}

		#endregion
	}
}
