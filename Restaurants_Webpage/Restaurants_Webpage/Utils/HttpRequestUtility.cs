using System.Net.Http;

namespace Restaurants_Webpage.Utils
{
    public enum HttpMethods { GET, POST, PUT, DELETE }
    public static class HttpRequestUtility
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _defaultResponseMessage = "Something went wrong, unable to perform such an action.";

        public static async Task<HttpResponseMessage?> SendRequestAsync(string url, HttpMethods method, JsonContent? jsonBody, Dictionary<string, string>? headers)
        {
            string methodType = "";
            switch (method)
            {
                case HttpMethods.GET:
                    if (jsonBody != null)
                    {
                        throw new Exception("Method GET must had empty body");
                    }
                    methodType = "GET";
                    break;

                case HttpMethods.POST:
                    methodType = "POST";
                    break;

                case HttpMethods.PUT:
                    methodType = "PUT";
                    break;

                case HttpMethods.DELETE:
                    if (jsonBody != null)
                    {
                        throw new Exception("Method DELETE must had empty body");
                    }
                    methodType = "DELETE";
                    break;

                default:
                    throw new Exception("Unspecified method");
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(methodType), url);
            if (jsonBody != null)
            {
                requestMessage.Content = jsonBody;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await _httpClient.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return responseMessage;
        }

        public static async Task<HttpResponseMessage?> SendSecureRequestJwtAsync(string url, HttpMethods method, JsonContent? jsonBody, string? jwt)
        {
            try
            {
                if (jwt == null)
                {
                    throw new Exception("Jwt can't be null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {jwt}"}
            };

            return await SendRequestAsync(url, method, jsonBody, headers);
        }

        public static async Task<string> GetResponseMessage(HttpResponseMessage response)
        {
            string? responseMessage = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseMessage))
            {
                return _defaultResponseMessage;
            }

            return responseMessage;
        }
    }
}
