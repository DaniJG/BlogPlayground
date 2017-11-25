using BlogPlayground.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.TagHelpers
{
    public class ProfilePictureTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly IActionContextAccessor actionAccessor;

        public ProfilePictureTagHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionAccessor)
        {
            this.urlHelperFactory = urlHelperFactory;
            this.actionAccessor = actionAccessor;
        }

        public ApplicationUser Profile { get; set; }
        public int? SizePx { get; set; }
        private bool IsDefaultPicture =>  String.IsNullOrWhiteSpace(this.Profile.PictureUrl);
        private IUrlHelper UrlHelper => this.urlHelperFactory.GetUrlHelper(this.actionAccessor.ActionContext);

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (this.Profile == null)
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
            img.TagRenderMode = TagRenderMode.SelfClosing;            
            img.Attributes.Add("title", this.GetAlternateText());
            img.Attributes.Add("src", this.GetPictureUrl());
            if (this.IsDefaultPicture && this.SizePx.HasValue) {
                img.Attributes.Add("style", $"height:{this.SizePx.Value}px;width:{this.SizePx.Value}px");
            }
            output.Content.SetHtmlContent(img);          
        }

        private string GetAlternateText()
        {            
            return this.Profile.FullName ?? this.Profile.UserName;
        }

        private string GetPictureUrl()
        {
            if (this.IsDefaultPicture)
            {
                return this.UrlHelper.Content("~/images/placeholder.png");
            }

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
