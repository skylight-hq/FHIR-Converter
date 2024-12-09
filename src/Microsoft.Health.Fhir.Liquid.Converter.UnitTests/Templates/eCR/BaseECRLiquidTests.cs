using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DotLiquid;
using Microsoft.Health.Fhir.Liquid.Converter.Models;
using Microsoft.Health.Fhir.Liquid.Converter.Utilities;
using Xunit;

namespace Microsoft.Health.Fhir.Liquid.Converter.UnitTests
{
    public class BaseConvertLiquidTemplate
    {

        protected void ConvertJsonWithLiquidTemplate(string templatePath, Dictionary<string, object> attributes, string expectedContent)
        {
            var templateContent = File.ReadAllText(templatePath);
            var template = TemplateUtility.ParseLiquidTemplate(templatePath, templateContent);
            Assert.True(template.Root.NodeList.Count > 0);

            // Template should be rendered correctly
            var templateProvider = new TemplateProvider(TestConstants.ECRTemplateDirectory, DataType.Ccda);
            var context = new Context(
                environments: new List<Hash>(),
                outerScope: new Hash(),
                registers: Hash.FromDictionary(new Dictionary<string, object>() { { "file_system", templateProvider.GetTemplateFileSystem() } }),
                errorsOutputMode: ErrorsOutputMode.Display,
                maxIterations: 0,
                formatProvider: CultureInfo.InvariantCulture,
                cancellationToken: CancellationToken.None);
            context.AddFilters(typeof(Filters));

            var codeContent = File.ReadAllText(Path.Join(TestConstants.ECRTemplateDirectory, "ValueSet", "ValueSet.json"));
            var codeMapping = TemplateUtility.ParseCodeMapping(codeContent);
            Console.WriteLine(codeMapping);
            if (codeMapping?.Root?.NodeList?.First() != null)
            {
                context["CodeMapping"] = codeMapping.Root.NodeList.First();
            }


            foreach (var keyValue in attributes)
            {
                context[keyValue.Key] = keyValue.Value;
            }

            var actualContent = template.Render(RenderParameters.FromContext(context, CultureInfo.InvariantCulture)).Trim().Replace("\n", " ").Replace("\t", "");

            // Many are harmless, but helpful for debugging
            foreach (var err in template.Errors)
            {
                Console.WriteLine(err.Message);
            }

            Assert.Equal(expectedContent, actualContent);
        }
    }
   
}
