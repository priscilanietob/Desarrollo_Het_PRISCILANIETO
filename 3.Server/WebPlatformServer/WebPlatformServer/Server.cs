//Priscila Nieto Burciaga 33599
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebPlatformServer
{
    public class Server
    {
        public int Port { get; set; } = 8080; 
        public string StaticFilesDirectory { get; set; }

        private TcpListener _listener;

        public Server(int port = 8080, string staticFilesDirectory = "static")
        {
            Port = port;
            StaticFilesDirectory = staticFilesDirectory; // FIXED: Use the parameter
            
            Console.WriteLine($"DEBUG: StaticFilesDirectory set to: {StaticFilesDirectory}");
            Console.WriteLine($"DEBUG: Full path: {Path.GetFullPath(StaticFilesDirectory)}");

            // Asegurar que el directorio existe
            if (!Directory.Exists(StaticFilesDirectory))
            {
                try
                {
                    Directory.CreateDirectory(StaticFilesDirectory);
                    Console.WriteLine($"Created directory: {StaticFilesDirectory}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Directory already exists: {StaticFilesDirectory}");
            }
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();
            Console.WriteLine($"Server started on port {Port}. Serving files from '{Path.GetFullPath(StaticFilesDirectory)}'");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }

        private void HandleClient(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                try
                {
                    var reader = new StreamReader(stream);
                    var requestLine = reader.ReadLine();

                    if (string.IsNullOrEmpty(requestLine))
                    {
                        SendResponse(stream, "400 Bad Request", "text/plain", "400 Bad Request: Empty request line");
                        return;
                    }

                    Console.WriteLine($"Request: {requestLine}");

                    var tokens = requestLine.Split(' ');
                    if (tokens.Length < 3)
                    {
                        SendResponse(stream, "400 Bad Request", "text/plain", "400 Bad Request: Invalid request line");
                        return;
                    }

                    var method = tokens[0];
                    var url = tokens[1];

                    if (method != "GET")
                    {
                        SendResponse(stream, "405 Method Not Allowed", "text/plain", "405 Method Not Allowed: Only GET method is supported");
                        return;
                    }

                    if (url == "/")
                        url = "/index.html";

                    // Prevenir directory traversal attacks
                    if (url.Contains(".."))
                    {
                        SendResponse(stream, "403 Forbidden", "text/plain", "403 Forbidden: Directory traversal not allowed");
                        return;
                    }

                    var filePath = Path.Combine(StaticFilesDirectory, url.TrimStart('/'));
                    filePath = Path.GetFullPath(filePath); // Obtener ruta absoluta

                    // Verificar que el archivo esté dentro del directorio permitido
                    var allowedPath = Path.GetFullPath(StaticFilesDirectory);
                    if (!filePath.StartsWith(allowedPath))
                    {
                        Console.WriteLine($"SECURITY: File outside allowed directory");
                        Console.WriteLine($"Requested: {filePath}");
                        Console.WriteLine($"Allowed: {allowedPath}");
                        SendResponse(stream, "403 Forbidden", "text/plain", "403 Forbidden: Access denied");
                        return;
                    }

                    Console.WriteLine($"Looking for: {filePath}");
                    Console.WriteLine($"File exists: {File.Exists(filePath)}");

                    if (!File.Exists(filePath))
                    {
                        // Show what files are available for debugging
                        Console.WriteLine("Available files:");
                        try
                        {
                            var files = Directory.GetFiles(StaticFilesDirectory, "*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                Console.WriteLine($"  - {file}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error listing files: {ex.Message}");
                        }

                        SendResponse(stream, "404 Not Found", "text/plain", $"404 Not Found: {url}");
                        return;
                    }

                    var content = File.ReadAllBytes(filePath);
                    var contentType = GetContentType(filePath);

                    Console.WriteLine($"Serving: {filePath} ({contentType})");
                    SendResponse(stream, "200 OK", contentType, content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling request: {ex.Message}");
                    SendResponse(stream, "500 Internal Server Error", "text/plain", "500 Internal Server Error");
                }
            }

            client.Close();
        }

        private void SendResponse(Stream stream, string status, string contentType, string content)
        {
            var contentBytes = Encoding.UTF8.GetBytes(content);
            SendResponse(stream, status, contentType, contentBytes);
        }

        private void SendResponse(Stream stream, string status, string contentType, byte[] contentBytes)
        {
            try
            {
                var header = $"HTTP/1.1 {status}\r\n" +
                             $"Content-Length: {contentBytes.Length}\r\n" +
                             $"Content-Type: {contentType}\r\n" +
                             $"Connection: close\r\n" +
                             $"\r\n";

                var headerBytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(contentBytes, 0, contentBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending response: {ex.Message}");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".html" => "text/html",
                ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}