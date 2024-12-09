using System.Collections.Generic;
using System.IO;
using DotLiquid;
using Xunit;

namespace Microsoft.Health.Fhir.Liquid.Converter.UnitTests
{
    public class ConvertContactPoint : BaseConvertLiquidTemplate
    {
        private static string path = Path.Join(TestConstants.ECRTemplateDirectory, "DataType", "_ContactPoint.liquid")
    
        [Fact]
        public void GivenNoAttributeReturnsEmpty()
        {
            ConvertJsonWithLiquidTemplate(path, new Dictionary<string, object>(), "");
        }

        [Fact]
        public void GivenTelValueReturnsPhone()
        {
            var attributes = new Dictionary<string, object>{{"ContactPoint", Hash.FromAnonymousObject(new { value = "tel:123" })}};
            ConvertJsonWithLiquidTemplate(
                path, 
                attributes, 
                @"""system"":""phone"", ""value"": ""123"", ""use"": """",");
        }

        [Fact]
        public void GivenTelValuAndUseReturnsPhone()
        {
            var attributes = new Dictionary<string, object>{{"ContactPoint", Hash.FromAnonymousObject(new { value = "tel:123", use="H" })}};
            ConvertJsonWithLiquidTemplate(
                path, 
                attributes, 
                @"""system"":""phone"", ""value"": ""123"", ""use"": ""home"",");
        }
    }
   
}
