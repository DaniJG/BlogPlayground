using BlogPlayground.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.TagHelpers
{
    public class ProfilePictureTagHelper : TagHelper
    {
        public ApplicationUser Profile { get; set; }
        public int? SizePx { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //Render nothing if there is no profile or profile doesn't have a picture url
            if (this.Profile == null || String.IsNullOrWhiteSpace(this.Profile.PictureUrl))
            {
                output.SuppressOutput();
                return;
            }
            
            //add wrapper span with well known class
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "profile-picture");

            //Add inner img element
            var img = new TagBuilder("img");
            var imgUri = new UriBuilder(this.Profile.PictureUrl);
            img.Attributes.Add("src", this.GetPictureUrl());
            output.Content.SetHtmlContent(img);          
        }

        private string GetPictureUrl()
        {
            var imgUriBuilder = new UriBuilder(this.Profile.PictureUrl);
            if (this.SizePx.HasValue)
            {
                var query = QueryString.FromUriComponent(imgUriBuilder.Query);
                query = query.Add("sz", this.SizePx.Value.ToString());
                imgUriBuilder.Query = query.ToString();
            }

            return imgUriBuilder.Uri.ToString();
        }
    }
}
