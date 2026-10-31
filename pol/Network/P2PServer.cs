using System.Net;
using System.Text;
using System.Text.Json;
using pol.Core.Models;
using pol.Data.Repositories;
using System.Linq;

namespace pol.Network
{
    public class P2PServer
    {
        private readonly INetMessageRepository _repo;
        private HttpListener _listener;
        private bool _isRunning;

        public P2PServer(INetMessageRepository repo)
        {
            _repo = repo;
        }

        public void Start(string ipAddress = "localhost", int port = 5000)
        {
            if (_isRunning) return;
            _isRunning = true;

            _listener = new HttpListener();
            // We use the IP address provided by the user
            _listener.Prefixes.Add($"http://{ipAddress}:{port}/");

            try
            {
                _listener.Start();
                Task.Run(() => ListenLoop());
            }
            catch (HttpListenerException ex)
            {
                // This usually happens if you try to bind to a non-local IP without Admin rights
                // or if the port is blocked.
                throw new Exception($"Failed to start listener. Are you running as Administrator?\nDetails: {ex.Message}");
            }
        }


        private async Task ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    // Handle each request in a separate task
                    _ = Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException)
                {
                    // Listener was stopped
                    break;
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                // Add basic CORS headers to allow browser/tools to connect easily
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                if (request.Url.AbsolutePath == "/messages")
                {
                    // ... inside HandleRequest ...
                    if (request.HttpMethod == "GET")
                    {
                        // 1. Parse 'since' parameter
                        long since = 0;
                        var sinceParam = request.QueryString["since"];
                        if (!string.IsNullOrEmpty(sinceParam)) long.TryParse(sinceParam, out since);

                        // 2. Parse 'limit' parameter (Default to 100 if not provided)
                        int limit = 100;
                        var limitParam = request.QueryString["limit"];
                        if (!string.IsNullOrEmpty(limitParam)) int.TryParse(limitParam, out limit);

                        // 3. Get messages using the NEW repository method
                        var messages = await _repo.GetMessagesSinceAsync(since, limit);

                        // 4. Convert to DTO
                        var dtos = messages.Select(m => MessageMapper.ToDto(m)).ToList();

                        // 5. Send JSON response
                        string json = JsonSerializer.Serialize(dtos);
                        byte[] buffer = Encoding.UTF8.GetBytes(json);

                        response.StatusCode = 200;
                        response.ContentType = "application/json";
                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    // ...
                    else if (request.HttpMethod == "POST")
                    {
                        // 1. Read JSON body
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string json = await reader.ReadToEndAsync();
                            var dto = JsonSerializer.Deserialize<MessageDto>(json);

                            if (dto != null)
                            {
                                // 2. Convert to NetMessage and Save
                                var msg = MessageMapper.FromDto(dto);
                                await _repo.AddMessageAsync(msg);
                                response.StatusCode = 201; // Created
                            }
                            else
                            {
                                response.StatusCode = 400; // Bad Request
                            }
                        }
                    }
                    else
                    {
                        response.StatusCode = 405; // Method Not Allowed
                    }
                }
                else
                {
                    response.StatusCode = 404; // Not Found
                }
            }
            catch (Exception)
            {
                response.StatusCode = 500; // Internal Server Error
            }
            finally
            {
                response.Close();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _listener?.Close();
        }
    }
}