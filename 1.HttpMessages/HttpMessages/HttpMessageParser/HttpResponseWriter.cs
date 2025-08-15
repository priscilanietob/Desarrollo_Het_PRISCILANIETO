using System;
using System.Collections.Generic;
using HttpMessageParser.Models;

namespace HttpMessageParser
{
    public class ResponseWriter : IResponseWriter
    {
        public string WriteResponse(HttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (string.IsNullOrWhiteSpace(response.Protocol))
                throw new ArgumentException("El protocolo no puede estar vacío.");
            if (response.StatusCode <= 0)
                throw new ArgumentException("El código de estado debe ser un número positivo.");
            if (string.IsNullOrWhiteSpace(response.StatusText))
                throw new ArgumentException("La frase de estado no puede estar vacía.");

            var responseLines = new List<string>
                {
                    $"{response.Protocol} {response.StatusCode} {response.StatusText}"
                };

            if (response.Headers != null)
            {
                foreach (var header in response.Headers)
                {
                    if (string.IsNullOrWhiteSpace(header.Key) || string.IsNullOrWhiteSpace(header.Value))
                        throw new ArgumentException("Los encabezados no pueden tener claves o valores vacíos.");
                    responseLines.Add($"{header.Key}: {header.Value}");
                }
            }

            responseLines.Add(""); 

            if (!string.IsNullOrEmpty(response.Body))
            {
                responseLines.Add(response.Body);
            }

            return string.Join("\n", responseLines);
        }
    }
}
