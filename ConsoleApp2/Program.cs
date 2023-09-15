using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ConsoleApp2;

namespace webscraping
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                int page = 1;
                while (true)
                {
                    string url = $"https://www.amazon.in/s?k=shoes&crid=318ONRA2ZX522&sprefix=shoe%2Caps%2C251&ref=nb_sb_noss_1&page={page}";
                    driver.Navigate().GoToUrl(url);
                    List<ProductInfo> items = new List<ProductInfo>();
                    var titles = driver.FindElements(By.CssSelector(".a-section.a-spacing-small.puis-padding-left-micro.puis-padding-right-micro"));
                    if (titles != null)
                    {   
                        foreach (var title in titles)
                        {
                            string name = title.FindElement(By.CssSelector(".a-size-mini.s-line-clamp-1")).Text;
                            Thread.Sleep(1000);
                            string price = title.FindElement(By.CssSelector(".a-price")).Text;
                            Thread.Sleep(1000);
                            string offer = title.FindElement(By.XPath("//span[contains(text(),'off')]")).Text;
                            items.Add(new ProductInfo { Name = name, Price = price, offer = offer });
                            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(price) && !string.IsNullOrWhiteSpace(offer))
                            {
                                Console.WriteLine("Name: " + name.Trim());
                                Console.WriteLine("Price: " + price.Trim());
                                Console.WriteLine("Offer:" + offer.Trim());
                            }
                        }
                    }
                    ExportToCSV(items);
                    try
                    {
                        var nextButton = driver.FindElement(By.XPath("//a[contains(text(),'Next')]"));
                        if (nextButton != null && nextButton.Displayed)
                        {
                            nextButton.Click();
                            page++; 
                        }
                        else
                        {
                            Console.WriteLine("No more pages available.");
                            break; 
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Next button not found.");
                        break; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

       /*static void ExportToCSV(List<ProductInfo> items)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var writer = new System.IO.StreamWriter("C:\\Users\\iray\\Desktop\\New folder (2)\\items.csv"))
            using (var csv = new CsvHelper.CsvWriter(writer, config))
            {
                csv.WriteRecords(items);
            }
        }*/
        static void ExportToCSV(List<ProductInfo> items)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,//!File.Exists("C:\\Users\\iray\\Desktop\\New folder (2)\\items.csv"), // Add this line to write header only if the file doesn't exist
            };

            using (var writer = new System.IO.StreamWriter("C:\\Users\\iray\\Desktop\\New folder (2)\\items.csv", true)) // Open the file in append mode by passing 'true'
            using (var csv = new CsvHelper.CsvWriter(writer, config))
            {
                csv.WriteRecords(items);
            }
        }

    }
}  
