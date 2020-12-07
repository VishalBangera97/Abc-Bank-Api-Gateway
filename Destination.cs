using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class Destination
    {
        public string Uri { get; set; }
        public bool RequiresAuthentication { get; set; }
        static HttpClient client = new HttpClient();
        public Destination(string uri, bool requiresAuthentication)
        {
            Uri = uri;
            RequiresAuthentication = requiresAuthentication;
        }
        public Destination(string uri) : this(uri, false) { }
        private Destination()
        {
            Uri = "/";
            RequiresAuthentication = false;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequest request)
        {
            string requestContent;
            using (Stream receiveStream = request.Body)
            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                requestContent = readStream.ReadToEnd();
            HttpRequestMessage newRequest = new HttpRequestMessage(new HttpMethod(request.Method), CreateDestinationUri(request));
            newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
            var response = await client.SendAsync(newRequest);
            return response;
        }

        private string CreateDestinationUri(HttpRequest request)
        {
            string requestPath = request.Path.ToString();
            string queryString = request.QueryString.ToString();
            string endpoint = "";
            string[] endpointSplit = requestPath.Substring(1).Split('/');
            if (endpointSplit.Length > 1)
                for (int i = 1; i < endpointSplit.Length; i++)
                    endpoint += "/" + endpointSplit[i];
            return Uri + endpoint + queryString;
        }
    }
}
