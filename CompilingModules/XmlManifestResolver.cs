using System;
using System.Reflection;
using System.Xml;

namespace XmlPrime.Samples.CompilingModules
{
	internal class XmlManifestResolver : XmlUrlResolver
	{
		#region Private Fields

		private readonly Assembly _assembly;

		#endregion

		#region Public Constructors

		public XmlManifestResolver(Assembly assembly)
		{
			_assembly = assembly;
		}

		#endregion

		#region Public Methods

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.Scheme != "assembly")
				return base.GetEntity(absoluteUri, role, ofObjectToReturn);

			const string root = "XmlPrime.Samples.CompilingModules.";
			var path = root + absoluteUri.AbsolutePath;

			var stream = _assembly.GetManifestResourceStream(path);

			return stream;
		}

		#endregion
	}
}
