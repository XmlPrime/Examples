using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace XmlPrime.Samples.XsltScript
{
    /// <summary>
    /// A simple example demonstrating use of a C# script called from an XSLT stylesheet.
    /// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/native-modules.htm">Native Modules</seealso>
    internal class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Performs the transformation.
        /// </summary>
        /// <param name="inputPath">The filename of the context item document.</param>
        /// <param name="xsltPath">The filename of the XSLT transformation.</param>
        /// <param name="outputPath">The filename of the primary output.</param>
        private static void PerformTransformation(string inputPath, string xsltPath, string outputPath)
        {
            Debug.Assert(inputPath != null, "inputPath in null");
            Debug.Assert(xsltPath != null, "xsltPath in null");
            Debug.Assert(inputPath != null, "outputPath in null");

            // We create an XsltSettings instance, whichh describes all the settings used for compilation.
            // In particular, we will enable use of scripts and set the context item type. 
            // By default the context item type is set to none, and so the context item cannot be set unless 
            // we override the type here. 
            var xsltSettings = new XsltSettings { ContextItemType = XdmType.Node, EnableScript = true };

            // We can then compile the transformation using the Compile method. 
            // This returns us an Xslt object encapsulating the transformation. 
            var xslt = Xslt.Compile(xsltPath, xsltSettings);

            // Now we have our transformation object we now just need to execute it.

            // In order to transform  the document we load it into an XdmDocument.
            var condextDocument = new XdmDocument(inputPath, XmlSpace.Preserve);

            // We will use the ApplyTemplates method to initiate a transformation by applying templates 
            // in the default mode and serializing the primary result document to a stream.
            using (var outputStream = File.Create(outputPath))
            {
                xslt.ApplyTemplates(condextDocument.CreateNavigator(), outputStream);
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Performs a transformation on a specified file and serializes the result to a specified file.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>0 if the transformation was executed successfully; otherwise -1.</returns>
        public static int Main(string[] args)
        {
            Debug.Assert(args != null, "args in null");

            if (args.Length != 3)
            {
                var command = Environment.GetCommandLineArgs()[0];
                var name = Path.GetFileNameWithoutExtension(command);

                Console.Error.WriteLine("Usage: {0} <input.xml> <transform.xsl> <output>", name);
                return -1;
            }

            var inputPath = args[0];
            var xsltPath = args[1];
            var outputPath = args[2];

            try
            {
                PerformTransformation(inputPath, xsltPath, outputPath);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return -1;
            }

            return 0;
        }

        #endregion
    }
}
