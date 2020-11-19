using System;
using System.Collections.Generic;
using Razor.Templating.Core;
using System.Threading.Tasks;
namespace Razor.Templating.Example.Invoice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var invoiceModel = new Templates.Invoice
            {
                InvoiceNumber = "3232",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                CompanyAddress = new Templates.Address
                {
                    Name = "XY Technologies",
                    AddressLine1 = "XY Street, Park Road",
                    City = "Chennai",
                    Country = "India",
                    Email = "xy-email@gmail.com",
                    PinCode = "600001"
                },
                BillingAddress = new Templates.Address
                {
                    Name = "XY Customer",
                    AddressLine1 = "ZY Street, Loyal Road",
                    City = "Bangalore",
                    Country = "India",
                    Email = "xy-customer@gmail.com",
                    PinCode = "343099"
                },
                PaymentMethod = new Templates.PaymentMethod
                {
                    Name = "Cheque",
                    ReferenceNumber = "94759849374"
                },
                LineItems = new List<Templates.LineItem>
        {
            new Templates.LineItem
            {
            Id = 1,
            ItemName = "USB Type-C Cable",
            Quantity = 3,
            PricePerItem = 10.33M
            },
               new Templates.LineItem
            {
            Id = 1,
            ItemName = "SSD-512G",
            Quantity = 10,
            PricePerItem = 90.54M
            }
        },
                CompanyLogoUrl = "https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png"
            };
            var invoiceHtml = await RazorTemplateEngine.RenderAsync("~/Invoice.cshtml", invoiceModel);
            Console.WriteLine(invoiceHtml);
            Console.ReadLine();
        }
    }
}
