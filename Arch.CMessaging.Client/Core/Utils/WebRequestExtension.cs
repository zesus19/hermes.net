using System;
using System.Net;

namespace Arch.CMessaging.Client.Core.Utils
{
    public static class WebRequestExtension
    {
        public static WebResponse BetterGetResponse(this WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    return wex.Response;
                }
                throw;
            }
        }
    }
}

