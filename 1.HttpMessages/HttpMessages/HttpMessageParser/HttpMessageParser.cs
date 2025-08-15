using System;
using System.Collections.Generic;
using System.Linq;
using HttpMessageParser.Models;

namespace HttpMessageParser
{
    public class HttpRequestParser : IRequestParser
    {
        public HttpRequest ParseRequest(string requestText)
        {
            if (requestText == null)
                throw new ArgumentNullException(nameof(requestText));

            if (string.IsNullOrWhiteSpace(requestText))
                throw new ArgumentException("La solicitud HTTP está vacía.");

            var lines = requestText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var firstLineParts = lines[0].Split(' ');
            if (firstLineParts.Length < 3)
                throw new ArgumentException("La solicitud HTTP no tiene formato válido.");

            string method = firstLineParts[0];
            string path = firstLineParts[1];
            string protocol = firstLineParts[2];

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("El método HTTP no puede estar vacío.");
            if (!path.Contains("/"))
                throw new ArgumentException("La ruta no es válida.");
            if (!protocol.StartsWith("HTTP"))
                throw new ArgumentException("Protocolo no válido.");

            var headers = new Dictionary<string, string>();
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) break;
                var headerParts = lines[i].Split(new[] { ':' }, 2);
                if (headerParts.Length != 2 || string.IsNullOrWhiteSpace(headerParts[0]) || string.IsNullOrWhiteSpace(headerParts[1]))
                    throw new ArgumentException("Formato de encabezado no válido.");
                headers[headerParts[0].Trim()] = headerParts[1].Trim();
            }

            var bodyIndex = Array.IndexOf(lines, "");
            string? body = bodyIndex >= 0 && bodyIndex < lines.Length - 1
                ? string.Join("\n", lines.Skip(bodyIndex + 1))
                : null; 

            return new HttpRequest
            {
                Method = method,
                RequestTarget = path,
                Protocol = protocol,
                Headers = headers,
                Body = body
            };
        }
    }
}