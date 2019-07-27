using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using UmbracoResumeWebsite.Helpers.Models;

namespace UmbracoResumeWebsite.Helpers
{
    public class Navigation
    {
        private IPublishedContent Content = null;
        private NavigationList NavigationList = null;
        private const string CACHE_KEY_PREFIX = "umbracoNavigationItems_";
        private double CacheExpiration = 0.0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publishedContent">The content object</param>
        /// <param name="cacheExpirationinMinutes">Cache expiration time in minutes</param>
        public Navigation(IPublishedContent publishedContent, double cacheExpirationinMinutes = 5.0)
        {
            Content = publishedContent;
            CacheExpiration = cacheExpirationinMinutes;
        }

        /// <summary>
        /// Get all navigation items
        /// </summary>
        /// <param name="getFromCache">Whether to get items from cache or not. Default is true.</param>
        public NavigationList GetItems(bool getFromCache = true)
        {
            NavigationList = new NavigationList();

            if (getFromCache)
            {
                var cacheKey = CACHE_KEY_PREFIX + Content.Key;
                NavigationList = GetItemsFromCache(cacheKey);
            }
            else
            {
                NavigationList = GetChildItems(Content);
            }
                
            return NavigationList;
        }

        private NavigationList GetChildItems(IPublishedContent parentContent)
        {
            var childItems = new NavigationList();
            childItems.Items = new List<NavigationListItem>();

            foreach (var childContent in parentContent.Children.Where(x => x.IsVisible()))
            {
                childItems.Items.Add(new NavigationListItem
                {
                    Id = childContent.Id,
                    Name = childContent.Name,
                    Url = childContent.Url,
                    Content = childContent,
                    List = childContent.Children.Any() ? GetChildItems(childContent) : new NavigationList()
                });
            }

            return childItems;
        }

        private NavigationList GetItemsFromCache(string cacheKey)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = new NavigationList();

            if (cache.Contains(cacheKey))
            {
                cachedObject = (NavigationList)cache[cacheKey];
            }
            else 
            {
                cachedObject = GetChildItems(Content);
                SetItemsToCache(cacheKey, cachedObject, CacheExpiration);
            }

            return cachedObject;
        }

        private void SetItemsToCache(string cacheKey, NavigationList cachedObject, double cacheExpiration)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();

            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpiration);
            cache.Set(cacheKey, cachedObject, policy);
        }
    }

}