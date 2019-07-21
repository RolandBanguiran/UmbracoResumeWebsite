using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace UmbracoResumeWebsite.Web.Models
{
    public class UmbracoNavigation
    {
        private IPublishedContent Content = null;
        private UmbracoNavigationList NavigationList = null;
        private const string CACHE_KEY_PREFIX = "umbracoNavigationItems_";
        private double CacheExpiration = 0.0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publishedContent">The content object</param>
        /// <param name="cacheExpirationinMinutes">Cache expiration time in minutes</param>
        public UmbracoNavigation(IPublishedContent publishedContent, double cacheExpirationinMinutes = 5.0)
        {
            Content = publishedContent;
            CacheExpiration = cacheExpirationinMinutes;
        }

        /// <summary>
        /// Get all navigation items
        /// </summary>
        /// <param name="getFromCache">Whether to get items from cache or not. Default is true.</param>
        public UmbracoNavigationList GetItems(bool getFromCache = true)
        {
            NavigationList = new UmbracoNavigationList();

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

        private UmbracoNavigationList GetChildItems(IPublishedContent parentContent)
        {
            var childItems = new UmbracoNavigationList();
            childItems.Items = new List<UmbracoNavigationListItem>();

            foreach (var childContent in parentContent.Children.Where(x => x.IsVisible()))
            {
                childItems.Items.Add(new UmbracoNavigationListItem
                {
                    Id = childContent.Id,
                    Name = childContent.Name,
                    Url = childContent.Url,
                    Content = childContent,
                    List = childContent.Children.Any() ? GetChildItems(childContent) : new UmbracoNavigationList()
                });
            }

            return childItems;
        }

        private UmbracoNavigationList GetItemsFromCache(string cacheKey)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = new UmbracoNavigationList();

            if (cache.Contains(cacheKey))
            {
                cachedObject = (UmbracoNavigationList)cache[cacheKey];
            }
            else 
            {
                cachedObject = GetChildItems(Content);
                SetItemsToCache(cacheKey, cachedObject, CacheExpiration);
            }

            return cachedObject;
        }

        private void SetItemsToCache(string cacheKey, UmbracoNavigationList cachedObject, double cacheExpiration)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();

            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheExpiration);
            cache.Set(cacheKey, cachedObject, policy);
        }
    }

    public class UmbracoNavigationList
    {
        public IList<UmbracoNavigationListItem> Items { get; set; }
    }

    public class UmbracoNavigationListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IPublishedContent Content { get; set; }
        public UmbracoNavigationList List { get; set; }

    }
}