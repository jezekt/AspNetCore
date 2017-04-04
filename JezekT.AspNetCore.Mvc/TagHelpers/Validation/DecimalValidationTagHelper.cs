using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Mvc.TagHelpers.Validation
{
    [HtmlTargetElement("decimal-validation")]
    public class DecimalValidationTagHelper : TagHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private const string BaseFolder = "lib";
        private const string LocaleNumberFilePattern = "cldr-numbers-full/main/{0}/numbers.json";


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            var sb = new StringBuilder();

            AppendScripts(sb);
            AppendInitializationScript(sb);

            output.Content.AppendHtml(sb.ToString());
        }


        public DecimalValidationTagHelper(IHostingEnvironment env)
        {
            if (env == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _hostingEnvironment = env;
        }


        private void AppendScripts(StringBuilder sb)
        {
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.16.0/jquery.validate.min.js\" asp-fallback-src=\"/{BaseFolder}/jquery-validation/dist/jquery.validate.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.6/jquery.validate.unobtrusive.min.js\" asp-fallback-src=\"/{BaseFolder}/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.8/cldr.min.js\" asp-fallback-src=\"/{BaseFolder}/cldrjs/dist/cldr.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.8/cldr/event.min.js\" asp-fallback-src=\"/{BaseFolder}/cldrjs/dist/cldr/event.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.8/cldr/supplemental.min.js\" asp-fallback-src=\"/{BaseFolder}/cldrjs/dist/cldr/supplemental.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/globalize/1.2.3/globalize.min.js\" asp-fallback-src=\"/{BaseFolder}/globalize/dist/globalize.js\"></script>");
            sb.AppendLine($"<script src=\"https://cdnjs.cloudflare.com/ajax/libs/globalize/1.2.3/globalize/number.min.js\" asp-fallback-src=\"/{BaseFolder}/globalize/dist/globalize/number.js\"></script>");
            sb.AppendLine($"<script src=\"/{BaseFolder}/jquery-validation-globalize/jquery.validate.globalize.js\"></script>");
        }

        private void AppendInitializationScript(StringBuilder sb)
        {
            sb.AppendLine("<script>");
                sb.AppendLine($"$.when($.get(\"/{BaseFolder}/cldr-core/supplemental/likelySubtags.json\"),");
                    sb.AppendLine($"$.get(\"/{BaseFolder}/cldr-core/supplemental/numberingSystems.json\"),");
                    sb.AppendLine($"$.get(\"/{GetLocaleNumberScript()}\"))");
                sb.AppendLine(".then(function () {");
                    sb.AppendLine("return [].slice.apply(arguments, [0]).map(function (result) {");
                        sb.AppendLine("return result[0];");
                    sb.AppendLine("});");
                sb.AppendLine("}).then(Globalize.load).then(function () {");
                    sb.AppendLine($"Globalize.locale(\"{CultureInfo.CurrentUICulture.Name}\");");
                sb.AppendLine("});");
            sb.AppendLine("</script>");
        }

        private string GetLocaleNumberScript()
        {
            var fileToUse = Path.Combine(BaseFolder, string.Format(LocaleNumberFilePattern, "en-GB")); //Default localisation to use
            var regionalisedFileFull = $"{BaseFolder}/{string.Format(LocaleNumberFilePattern, CultureInfo.CurrentUICulture.Name)}";
            var regionalisedFileTwoLetter = $"{BaseFolder}/{string.Format(LocaleNumberFilePattern, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)}";

            if (File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, regionalisedFileFull)))
            {
                fileToUse = regionalisedFileFull;
            }
            else if (File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, regionalisedFileTwoLetter)))
            {
                fileToUse = regionalisedFileTwoLetter;
            }
            
            return fileToUse;
        }
    }
}
