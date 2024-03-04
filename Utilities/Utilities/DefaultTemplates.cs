using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;
using Utilities.Models;

namespace Utilities.Utilities
{
    public static class DefaultTemplates
    {
        public static string GetEmailTemplate(EmailTypeEnums emailType)
        {
            string rawContent = DelimiterResolver.AppendDelimiters(emailType.ToString());
            List<CustomTag> defaultTags = CustomTagExtension.DefaultTemplateEmailTypeTags();
            CustomTag customTag = defaultTags.Where(
                r => r.TagName.Equals(emailType.ToString(),
                StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return new CustomTagsResolver()
            {
                RawContent = rawContent,
                StartPattern = customTag.TagNameWithStartPattern,
                StartDelimiterLength = customTag.StartDelimiterLength,
                TemplateContentpath = customTag.ContentPath,
            }.FinalContent;
        }

        public static string GetEmailSubject(EmailTypeEnums emailType)
        {
            string rawContent = DelimiterResolver.AppendDelimiters(emailType.ToString());
            List<CustomTag> defaultTags = CustomTagExtension.DefaultSubjectEmailTypeTags();
            CustomTag customTag = defaultTags.Where(
                r => r.TagName.Equals(emailType.ToString(),
                StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return new CustomTagsResolver()
            {
                RawContent = rawContent,
                StartPattern = customTag.TagNameWithStartPattern,
                StartDelimiterLength = customTag.StartDelimiterLength,
                TemplateContentpath = customTag.ContentPath,
            }.FinalContent;
        }
    }
    
}
