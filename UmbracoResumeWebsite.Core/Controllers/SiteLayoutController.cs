using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoResumeWebsite.Core.Models;

namespace UmbracoResumeWebsite.Core.Controllers
{
    public class SiteLayoutController : SurfaceController
    {
        public ActionResult RenderHeader()
        {
            return PartialView("Layout/_Header");
        }

        public ActionResult RenderMainNavigation()
        {
            var root = Umbraco.ContentAtRoot().First();
            var navigation = new UmbracoNavigation(root);

            return PartialView("Layout/_Navigation", navigation.GetItems(false));
        }
    }
}
