using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NumericSeries.Models;
using NumericSeries.Models.Series;
using System;

namespace NumericSeries.Controllers
{
    public class SeriesController : Controller
    {
        [HttpGet("/series/{series}/{n:int}")]
        public IActionResult Index(string series, int n = 0)
        {
            if (n < 0)
                return BadRequest("Error 400: El índice n no puede ser negativo.");

            Func<int, long> serieFunc;

            switch (series.ToLower())
            {
                case "natural":
                    serieFunc = x => Natural.GetValue(x);
                    break;
                case "primos":
                    serieFunc = x => Primos.GetValue(x);
                    break;
                case "fibonacci":
                    serieFunc = x => Fibonacci.GetValue(x);
                    break;
                case "factorial":
                    serieFunc = x => Factorial.GetValue(x);
                    break;
                case "pares":
                    serieFunc = x => Pares.GetValue(x);
                    break;
                default:
                    return NotFound($"Error 404: La serie '{series}' no está implementada.");
            }

            long result = serieFunc(n);

            return View(new SeriesViewModel
            {
                Series = series.ApplyCase(LetterCasing.Sentence),
                N = n,
                Result = result
            });
        }
    }
}
