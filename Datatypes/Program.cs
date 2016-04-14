using System;
using System.Diagnostics;
using System.IO;

namespace XmlPrime.Samples.Datatype
{
    /// <summary>
    /// A simple example demonstrating use of the an XML Schema datatype in an XPath expression.
    /// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xpath.htm">Using XPath 2.0</seealso>
    internal class Program
    {
        #region Public Static Methods

        /// <summary>
        /// Performs a query on a specified file and serializes the result to a specified file.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>0 if the results of the query was serialized successfully; otherwise -1.</returns>
        public static int Main(string[] args)
        {
            Debug.Assert(args != null, "args in null");

            if (args.Length != 0)
            {
                var command = Environment.GetCommandLineArgs()[0];
                var name = Path.GetFileNameWithoutExtension(command);

                Console.Error.WriteLine("Usage: {0}", name);
                return -1;
            }

            // In order to describe how the query should be compiled we need set up an XPathSettings object. 
            // This describes all the settings used for compilation. 
            var xpathSettings = new XPathSettings();

            // By default, XPath does not bind the 'xs' prefix to the XML Schema namespace URI, so we binding it here.
            xpathSettings.DeclareNamespace("xs", XmlNamespaces.XmlSchemaNamespace);

            const string pathExpression = "xs:date('2012-01-01') > current-date()";

            // We can then compile the expression using the Compile method. 
            // This returns us an XPath object encapsulating the query.
            var xpath = XPath.Compile(pathExpression, xpathSettings);

            // We call the XPath.EvaluateToItem method to evaluate the expression.
            // This returns a single item.
            var result = xpath.EvaluateToItem();

            Console.Write(pathExpression);
            Console.Write(" evaluates to ");
            Console.WriteLine(result);

            return 0;
        }

        #endregion
    }
}
