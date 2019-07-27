using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace UmbracoResumeWebsite.Helpers.Models
{
    public class NavigationListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IPublishedContent Content { get; set; }
        public NavigationList List { get; set; }

    }
}