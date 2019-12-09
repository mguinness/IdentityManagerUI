using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagerUI
{
    internal class ModalContext
    {
        public IHtmlContent Body { get; set; }
        public IHtmlContent Footer { get; set; }
    }

    public enum ModalSize { Default, Small, Large }

    [RestrictChildren("modal-body", "modal-footer")]
    public class ModalTagHelper : TagHelper
    {
        public ModalSize Size { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var modalContext = new ModalContext();
            context.Items.Add(typeof(ModalTagHelper), modalContext);
            await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.SetAttribute("role", "dialog");
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("aria-labelledby", $"Label_{context.UniqueId}");
            output.Attributes.SetAttribute("tabindex", "-1");
            output.Attributes.SetAttribute("aria-hidden", "true");

            var classNames = "modal fade";
            if (output.Attributes.ContainsName("class"))
                classNames = String.Concat(output.Attributes["class"].Value, " ", classNames);
            output.Attributes.SetAttribute("class", classNames);

            var size = Size == ModalSize.Small ? "modal-sm" : (Size == ModalSize.Large ? "modal-lg" : String.Empty);
            var template = $@"<div class='modal-dialog {size}' role='document'>
                <div class='modal-content'>
                    <div class='modal-header'>
                        <h4 class='modal-title' id='Label_{context.UniqueId}'>{Title}</h4>
                        <button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>
                    </div>";
            output.Content.AppendHtmlLine("\n" + template);

            output.Content.AppendHtml("<div class='modal-body'>");
            if (modalContext.Body != null)
                output.Content.AppendHtml(modalContext.Body);
            output.Content.AppendHtmlLine("</div>");

            if (modalContext.Footer != null)
            {
                output.Content.AppendHtml("<div class='modal-footer'>");
                output.Content.AppendHtml(modalContext.Footer);
                output.Content.AppendHtmlLine("</div>");
            }

            output.Content.AppendHtmlLine("</div>\n</div>");
        }
    }

    [HtmlTargetElement("modal-body", ParentTag = "modal")]
    public class ModalBodyTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var modalContext = (ModalContext)context.Items[typeof(ModalTagHelper)];
            modalContext.Body = childContent;
            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("modal-footer", ParentTag = "modal")]
    public class ModalFooterTagHelper : TagHelper
    {
        public string DismissText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var modalContext = (ModalContext)context.Items[typeof(ModalTagHelper)];

            var footerContent = new DefaultTagHelperContent();
            footerContent.AppendHtml(childContent);
            if (DismissText != null)
                footerContent.AppendFormat("<button type='button' class='btn btn-default' data-dismiss='modal'>{0}</button>\n", DismissText);

            modalContext.Footer = footerContent;
            output.SuppressOutput();
        }
    }
}
