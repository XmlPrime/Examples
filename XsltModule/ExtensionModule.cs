using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace XmlPrime.Samples.XsltModule
{
    [XdmModule("http://www.xmlprime.com/xpath")]
    public class ExtensionModule
    {
        #region Private Static Methods

        private static decimal[] BinaryToOctents(byte[] arg)
        {
            var length = arg.Length;
            var decimals = new decimal[length];
            for (var i = 0; i < length; ++i)
                decimals[i] = arg[i];
            return decimals;
        }
        
        private static string BinaryToString(byte[] arg, string encoding)
        {
            var enc = System.Text.Encoding.GetEncoding(encoding);
            if (enc == null)
                throw new ArgumentNullException("encoding");

            return enc.GetString(arg);
        }

        private static byte[] StringToBinary(string arg, string encoding)
        {
            var enc = System.Text.Encoding.GetEncoding(encoding);
            if (enc == null)
                throw new ArgumentNullException("encoding");

            return enc.GetBytes(arg);
        }

        #endregion
        /*
        [XdmFunction("base64Binary-to-octets")]
        [XdmType(XmlTypeCode.Integer, Quantifier.ZeroOrMore)] 
        public static decimal[] Base64BinaryToOctets([XdmType(XmlTypeCode.Base64Binary, Quantifier.ZeroOrMore)] byte[] arg)
        {
            return BinaryToOctents(arg);
        }

        [XdmFunction("base64Binary-to-string")]
        public static string Base64BinaryTostring([XdmType(XmlTypeCode.Base64Binary, Quantifier.ZeroOrMore)] byte[] arg, string encoding)
        {
            return BinaryToString(arg, encoding);
        }

        [XdmFunction("hexBinary-to-octets")]
        [XdmType(XmlTypeCode.Integer, Quantifier.ZeroOrMore)]
        public static decimal[] HexBinaryToOctets([XdmType(XmlTypeCode.Base64Binary, Quantifier.ZeroOrMore)] byte[] arg)
        {
            return BinaryToOctents(arg);
        }

        [XdmFunction("hexBinary-to-string")]
        public static string HexBinaryTostring([XdmType(XmlTypeCode.Base64Binary, Quantifier.ZeroOrMore)] byte[] arg, string encoding)
        {
            return BinaryToString(arg, encoding);
        }

        [XdmFunction("string-to-hexBinary")]
        [XdmType(XmlTypeCode.HexBinary, Quantifier.ZeroOrMore)]
        public static byte[] StringToHexBinary(string arg, string encoding)
        {
            return StringToBinary(arg, encoding);
        }

        [XdmFunction("string-to-base64Binary")]
        [XdmType(XmlTypeCode.Base64Binary, Quantifier.ZeroOrMore)]
        public static byte[] StringToBase64Binary(string arg, string encoding)
        {
            return StringToBinary(arg, encoding);
        }
        */

        [XdmFunction("string-to-utf8")]
        [XdmType(XmlTypeCode.Integer, Quantifier.ZeroOrMore)]
        public static IEnumerator<decimal> StringToUtf8(string arg)
        {
            foreach (var b in StringToBinary(arg, "utf-8"))
                yield return b;
        }
    }
}
