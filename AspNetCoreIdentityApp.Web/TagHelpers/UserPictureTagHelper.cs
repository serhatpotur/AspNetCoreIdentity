using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserPictureTagHelper : TagHelper
    {
        public string? PictureUrl { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            if (String.IsNullOrEmpty(PictureUrl)) {
                output.Attributes.SetAttribute("src", "/UserPictures/defaultimage.jpg");
                output.Attributes.SetAttribute("width", "100");
                output.Attributes.SetAttribute("height", "100");
            }

            else
            {
                output.Attributes.SetAttribute("src", $"/UserPictures/{PictureUrl}");
                output.Attributes.SetAttribute("width", "100");
                output.Attributes.SetAttribute("height", "100");

            }


            //base.Process(context, output);
        }
    }
}
