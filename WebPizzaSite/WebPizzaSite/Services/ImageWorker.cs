using SixLabors.ImageSharp; // Для роботи з зображеннями
using SixLabors.ImageSharp.Processing; // Для обробки зображень
using SixLabors.ImageSharp.Formats; // Для форматів зображень // Для роботи з форматом WebP
using Microsoft.AspNetCore.Hosting;
using WebPizzaSite.Interfaces;
using System.Net.Http;
using SixLabors.ImageSharp.Formats.Webp;

namespace WebPizzaSite.Services
{
    public class ImageWorker : IImageWorker
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageWorker(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string ImageSave(string url)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp"; // Змінено на webp
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Надсилаємо GET запит до URL зображення
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Перевіряємо, чи статус код відповіді вказує на успіх (наприклад, 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Визначаємо шлях для збереження зображення
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageName);
                        var dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir) && dir != null)
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Завантажуємо байти зображення
                        byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;

                        // Використовуємо ImageSharp для збереження зображення у форматі WebP
                        using (var image = Image.Load(imageBytes)) // Завантаження зображення
                        {
                            // Можна виконати обробку зображення, якщо потрібно
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(800, 800), // Вкажіть розміри, якщо потрібно
                                Mode = ResizeMode.Max // Режим масштабування
                            }));

                            // Зберігаємо зображення у форматі WebP
                            image.Save(path, new WebpEncoder());
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Не вдалося отримати зображення. Код статусу: {response.StatusCode}");
                        return String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
                return String.Empty;
            }
            return imageName;
        }
    }
}
