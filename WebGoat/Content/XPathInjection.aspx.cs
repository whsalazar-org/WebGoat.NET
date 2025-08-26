using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OWASP.WebGoat.NET
{
    public partial class XPathInjection : System.Web.UI.Page
    {
        // Make into actual lesson
        private string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><sales><salesperson><name>David Palmer</name><city>Portland</city><state>or</state><ssn>123-45-6789</ssn></salesperson><salesperson><name>Jimmy Jones</name><city>San Diego</city><state>ca</state><ssn>555-45-6789</ssn></salesperson><salesperson><name>Tom Anderson</name><city>New York</city><state>ny</state><ssn>444-45-6789</ssn></salesperson><salesperson><name>Billy Moses</name><city>Houston</city><state>tx</state><ssn>333-45-6789</ssn></salesperson></sales>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["state"] != null)
            {
                FindSalesPerson(Request.QueryString["state"]);
            }
        }

        private void FindSalesPerson(string state)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            // Prepare XPath with variable
            XPathNavigator navigator = xDoc.CreateNavigator();
            XPathExpression expr = navigator.Compile("//salesperson[state=$state]");

            // Prepare argument list
            XsltArgumentList argList = new XsltArgumentList();
            argList.AddParam("state", "", state);

            // Use custom context for variables
            var context = new XsltContextImpl(argList);
            expr.SetContext(context);

            XPathNodeIterator iterator = navigator.Select(expr);
            if (iterator.Count > 0)
            {
                // Your processing logic here
            }

        }
    }

    // Minimal XsltContext implementation for variable resolution
    public class XsltContextImpl : XsltContext
    {
        private readonly XsltArgumentList args;
        public XsltContextImpl(XsltArgumentList arguments)
        {
            this.args = arguments;
        }
        // For variable usage only
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            object val = args.GetParam(name, "");
            return new XsltVariable(val);
        }
        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes) { return null; }
        public override bool Whitespace { get { return true; } }
        public override bool PreserveWhitespace(XPathNavigator node) { return true; }
        public override int CompareDocument(string baseUri, string nextbaseUri) { return 0; }
    }
    public class XsltVariable : IXsltContextVariable
    {
        private readonly object _value;
        public XsltVariable(object value)
        {
            _value = value;
        }
        public object Evaluate(XsltContext xsltContext)
        {
            return _value;
        }
        public bool IsLocal { get { return true; } }
        public bool IsParam { get { return true; } }
        public XPathResultType VariableType { get { return XPathResultType.Any; } }
    }
}
