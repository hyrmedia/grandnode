using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using Grand.Core;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure;
using Grand.Core.Domain.Customers;

namespace Grand.Services.Helpers
{
    /// <summary>
    /// User agent helper
    /// </summary>
    public partial class UserAgentHelper : IUserAgentHelper
    {
        private readonly GrandConfig _config;
        private readonly HttpContextBase _httpContext;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="httpContext">HTTP context</param>
        public UserAgentHelper(GrandConfig config, HttpContextBase httpContext)
        {
            this._config = config;
            this._httpContext = httpContext;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
        {
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            //no database created
            if (String.IsNullOrEmpty(_config.UserAgentStringsPath))
                return null;

            var filePath = CommonHelper.MapPath(_config.UserAgentStringsPath);
            var bowscapXmlHelper = new BrowscapXmlHelper(filePath);

            Singleton<BrowscapXmlHelper>.Instance = bowscapXmlHelper;
            return Singleton<BrowscapXmlHelper>.Instance;
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine()
        {
            if (_httpContext == null)
                return false;

            //we put required logic in try-catch block
            try
            {
                var bowscapXmlHelper = GetBrowscapXmlHelper();

                //we cannot load parser
                if (bowscapXmlHelper == null)
                    return false;

                var userAgent = _httpContext.Request.UserAgent;
                return bowscapXmlHelper.IsCrawler(userAgent);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }

        public virtual bool IsWebApi()
        {
            if (_httpContext == null)
                return false;
            try
            {
                var userAgent = _httpContext.Request.UserAgent;
                return userAgent == SystemCustomerNames.WebApi;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }

    }
}