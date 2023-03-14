using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SynologyAPI
{
    /// <summary>
    /// Synology API client provides authentication and API query interface for other clients.
    /// </summary>
    /// <remarks>
    /// API documentation here: https://global.synologydownload.com/download/Document/Software/DeveloperGuide/Package/FileStation/All/enu/Synology_File_Station_API_Guide.pdf
    /// </remarks>
    public abstract class SynologyClient : IDisposable
    {
        private const string DefaultSession = "Default";

        protected readonly string _host;
        protected readonly HttpClient _client;
        protected string _sid;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="host">Host - e.g. https://host:5001.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SynologyClient(string host)
        {
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            if (!Uri.IsWellFormedUriString(host, UriKind.Absolute)) throw new ArgumentException(nameof(host));

            this._client = new HttpClient();
        }

        /// <summary>
        /// Sign in.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="session">Session name.</param>
        public async Task SignInAsync(string userName, string password, string session = null)
        {
            if (!string.IsNullOrEmpty(_sid))
                throw new InvalidOperationException("Already logged in!");

            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(session)) session = DefaultSession;

            const string SynoApiAuth = "SYNO.API.Auth";
            const string SynoApiMethodLogin = "login";

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiAuth);
            JsonNode authApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiAuth];

            // although the API says min version is 1, versions 1 and 2 do not work, version 3 is the minimum supported one
            int authApiVersion = authApi["maxVersion"].GetValue<int>();
            string authApiEndpoint = authApi["path"].GetValue<string>();

            // this call will set auth cookie id:SID
            string uri = $"{_host}/webapi/{authApiEndpoint}?api={SynoApiAuth}&method={SynoApiMethodLogin}&version={authApiVersion}&account={Uri.EscapeDataString(userName)}&passwd={Uri.EscapeDataString(password)}&session={Uri.EscapeDataString(session)}&format=sid";
            string authApiResponse = await GetAsync(uri);

            JsonNode authResponse = JsonNode.Parse(authApiResponse);

            if (authResponse["success"].GetValue<bool>())
            {
                _sid = authResponse["data"]["sid"].GetValue<string>();
            }
            else 
            { 
                throw new UnauthorizedAccessException();
            }
        }

        /// <summary>
        /// Sign out.
        /// </summary>
        /// <param name="session">Session name.</param>
        public async Task SignOutAsync(string session = null)
        {
            if(string.IsNullOrEmpty(_sid))
                throw new InvalidOperationException("Not logged in!");

            if (string.IsNullOrEmpty(session)) session = DefaultSession;

            const string SynoApiAuth = "SYNO.API.Auth";
            const string SynoApiMethodLogout = "logout";

            // first query the endpoint on which the API can be called
            string queryApiResponse = await QueryApiAsync(SynoApiAuth);
            JsonNode authApi = JsonNode.Parse(queryApiResponse)["data"][SynoApiAuth];

            // although the API says min version is 1, versions 1 and 2 do not work, version 3 is the minimum supported one
            int authApiVersion = authApi["maxVersion"].GetValue<int>();
            string authApiEndpoint = authApi["path"].GetValue<string>();

            string uri = $"{_host}/webapi/{authApiEndpoint}?api={SynoApiAuth}&method={SynoApiMethodLogout}&version={authApiVersion}&session={Uri.EscapeDataString(session)}&_sid={_sid}";
            string authApiResponse = await GetAsync(uri);

            JsonNode authResponse = JsonNode.Parse(authApiResponse);
            if (authResponse["success"].GetValue<bool>())
            {
                _sid = null;
            }
            else
            {
                throw new Exception("Logout failed!");
            }
        }

        /// <summary>
        /// Query the API and get the supported version and the endpoint to be called.
        /// </summary>
        /// <param name="query">API name.</param>
        /// <returns>JSON response with the API info.</returns>
        public Task<string> QueryApiAsync(string query = null)
        {
            // TODO: add cache here
            string uri = $"{_host}/webapi/query.cgi?api=SYNO.API.Info&version=1&method=query";

            if (!string.IsNullOrEmpty(query))
                uri += $"&query={query}";

            return GetAsync(uri);
        }

        protected async Task<string> GetAsync(string uri)
        {
            using (var response = await _client.GetAsync(new Uri(uri)))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        protected async Task<string> PostAsync(string uri, Dictionary<string, string> content)
        {
            using (var formContent = new FormUrlEncodedContent(content))
            {
                using (var response = await _client.PostAsync(new Uri(uri), formContent))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        #region IDisposable implementation

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable implementation
    }
}
