using Microsoft.AspNetCore.Mvc;
using WeatherWebApp.Services;
using WeatherWebApp.Models;
using WeatherWebApp.Repos;

namespace WeatherWebApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;
        private readonly ExportService _exportService;

        public WeatherController(WeatherService weatherService, ExportService exportService)
        {
            _weatherService = weatherService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Index(string? city = null)
        {
            // Default to Johannesburg if no city specified
            city = string.IsNullOrEmpty(city) ? "Johannesburg" : city;
            
            try
            {
                var model = await _weatherService.GetWeatherForecast(city);
                return View(model);
            }
            catch (Exception ex)
            {
                // Log error and return error view
                ViewBag.ErrorMessage = $"Error fetching weather data: {ex.Message}";
                return View(new WeatherViewModel { City = city });
            }
        }

        public IActionResult ExportWeatherToCsv()
        {
            try
            {
                var weatherRepo = new WeatherRepo();
                var weatherData = weatherRepo.GetAll().Result;
                
                var fileName = $"weather_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToCsvManually(weatherData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting weather data: {ex.Message}");
            }
        }

        public IActionResult ExportWeatherToExcel()
        {
            try
            {
                var weatherRepo = new WeatherRepo();
                var weatherData = weatherRepo.GetAll().Result;
                
                var fileName = $"weather_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToExcel(weatherData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting weather data: {ex.Message}");
            }
        }

        public IActionResult ExportCitiesToCsv()
        {
            try
            {
                var cityRepo = new CityRepo();
                var cityData = cityRepo.GetAll().Result;
                
                var fileName = $"cities_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToCsvManually(cityData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting cities data: {ex.Message}");
            }
        }

        public IActionResult ExportCitiesToExcel()
        {
            try
            {
                var cityRepo = new CityRepo();
                var cityData = cityRepo.GetAll().Result;
                
                var fileName = $"cities_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToExcel(cityData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting cities data: {ex.Message}");
            }
        }

        public IActionResult ExportForecastToCsv()
        {
            try
            {
                var forecastRepo = new ForecastRepo();
                var forecastData = forecastRepo.GetAll().Result;
                
                var fileName = $"forecast_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToCsvManually(forecastData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting forecast data: {ex.Message}");
            }
        }

        public IActionResult ExportForecastToExcel()
        {
            try
            {
                var forecastRepo = new ForecastRepo();
                var forecastData = forecastRepo.GetAll().Result;
                
                var fileName = $"forecast_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);
                
                _exportService.ExportListToExcel(forecastData, filePath);
                
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting forecast data: {ex.Message}");
            }
        }
    }
}