using HttpMessageParser.Models;

namespace HttpMessageParser.Tests
{
    public class RequestParserTests
    {
        private IRequestParser requestParser;

        [SetUp]
        public void Setup()
        {
            // Replace the following with an assignment of your actual implementation of IRequestParser
            requestParser = new HttpRequestParser();
        }

        [Test]
        public void ParseRequest_EmptyRequestThrowsException()
        {
            string request = "";
            Assert.Throws<ArgumentException>(() => requestParser.ParseRequest(request));
        }

        [Test]
        public void ParseRequest_NullRequestThrowsException()
        {
            string request = null;
            Assert.Throws<ArgumentNullException>(() => requestParser.ParseRequest(request));
        }

        [Test]
        public void ParseRequest_InvalidRequestThrowsException()
        {
            string request = "INVALID REQUEST FORMAT";
            Assert.Throws<ArgumentException>(() => requestParser.ParseRequest(request));
        }

        [TestCase("GET", "/some/path", null)]
        [TestCase("POST", null, "HTTP/1.1")]
        [TestCase(null, "/some/path", "HTTP/1.1")]
        public void ParseRequest_RequestMissingRequiredValuesThrowsException(string? method, string? requestTarget, string? protocol)
        {
            string request = $"{method} {requestTarget} {protocol}\n";
            Assert.Throws<ArgumentException>(() => requestParser.ParseRequest(request));
        }

        [Test]
        public void ParseRequest_CorrectlyParsesMinimalRequest()
        {
            // Define un request HTTP mínimo válido
            string request = "GET /some/path HTTP/1.1\r\n";

            // Llama al método ParseRequest y obtiene el resultado
            HttpRequest result = requestParser.ParseRequest(request);

            // Verifica que el método HTTP sea "GET"
            Assert.That(result.Method, Is.EqualTo("GET"), "El método HTTP no coincide.");

            // Verifica que el target de la solicitud sea "/some/path"
            Assert.That(result.RequestTarget, Is.EqualTo("/some/path"), "El target de la solicitud no coincide.");

            // Verifica que el protocolo sea "HTTP/1.1"
            Assert.That(result.Protocol, Is.EqualTo("HTTP/1.1"), "El protocolo HTTP no coincide.");

            // Verifica que los headers no sean nulos y estén vacíos
            Assert.That(result.Headers, Is.Not.Null, "Los headers no deberían ser nulos.");
            Assert.That(result.Headers.Count, Is.EqualTo(0), "Los headers deberían estar vacíos.");

            // Verifica que el cuerpo de la solicitud sea nulo
            Assert.That(result.Body, Is.Null, "El cuerpo de la solicitud debería ser nulo.");
        }

        [Test]
        public void ParseRequest_CorrectlyParsesRequestWithHeaders()
        {
            string request = "POST /submit HTTP/1.1\n" +
                             "Host: example.com\n" +
                             "User-Agent: some-user-agent";

            HttpRequest result = requestParser.ParseRequest(request);

            Assert.That(result.Method, Is.EqualTo("POST"));
            Assert.That(result.RequestTarget, Is.EqualTo("/submit"));
            Assert.That(result.Protocol, Is.EqualTo("HTTP/1.1"));
            Assert.That(result.Headers, Is.Not.Null);
            Assert.That(result.Headers.Count, Is.EqualTo(2));
            Assert.That(result.Headers["Host"], Is.EqualTo("example.com"));
            Assert.That(result.Headers["User-Agent"], Is.EqualTo("some-user-agent"));
        }

        [Test]
        public void ParseRequest_CorrectlyParsesRequestWithBody()
        {
            string request = "PUT /update HTTP/1.1\n" +
                             "Content-Type: application/json\n" +
                             "Content-Length: 27\n" +
                             "\n" +
                             "{\"key\": \"value\"}";

            HttpRequest result = requestParser.ParseRequest(request);

            Assert.That(result.Method, Is.EqualTo("PUT"));
            Assert.That(result.RequestTarget, Is.EqualTo("/update"));
            Assert.That(result.Protocol, Is.EqualTo("HTTP/1.1"));
            Assert.That(result.Headers, Is.Not.Null);
            Assert.That(result.Headers.Count, Is.EqualTo(2));
            Assert.That(result.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(result.Headers["Content-Length"], Is.EqualTo("27"));
            Assert.That(result.Body, Is.EqualTo("{\"key\": \"value\"}"));
        }
    }
}
