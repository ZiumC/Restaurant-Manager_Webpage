using System.Net.Http;

namespace Restaurants_Webpage.Utils
{
    public enum HttpMethods { GET, POST, PUT, DELETE }
    public static class HttpRequestUtility
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<HttpResponseMessage?> SendRequestAsync(string url, HttpMethods method, JsonContent? jsonBody)
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


            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await httpClient.SendAsync(requestMessage);
            } 
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return responseMessage;
        }
    }
}
