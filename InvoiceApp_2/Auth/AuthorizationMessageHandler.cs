using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace MyInvoiceApp.Auth
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (InvalidOperationException)
            {
                // LocalStorage not available during prerendering
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}