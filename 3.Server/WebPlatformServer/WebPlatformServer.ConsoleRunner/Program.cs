using System;
using System.IO;

namespace WebPlatformServer.ConsoleRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Use the ABSOLUTE path to your 2.Presentation folder
                string webPath = @"C:\Users\prisc\OneDrive\Documentos\GitHub\Desarrollo_Het_PRISCILANIETO\2.Presentation\styling\index.html";
                
                Console.WriteLine($"Using directory: {webPath}");
                Console.WriteLine($"Directory exists: {Directory.Exists(webPath)}");

                if (!Directory.Exists(webPath))
                {
                    Console.WriteLine("ERROR: Directory does not exist!");
                    return;
                }

                // Show what's in the directory
                Console.WriteLine("\nDirectory contents:");
                var files = Directory.GetFiles(webPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    Console.WriteLine($"  {file}");
                }

                Server server = new Server(port: 8080, staticFilesDirectory: webPath);
                Console.WriteLine($"\nServer started at http://localhost:8080/");
                
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}