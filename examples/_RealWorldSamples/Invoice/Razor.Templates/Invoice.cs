using System;
using System.Collections.Generic;
using System.Linq;
namespace Razor.Templates
{
    public class Invoice
    {
	public string InvoiceNumber { get; set; }
	public DateTime CreatedDate { get; set; }
	public DateTime DueDate { get; set; }
	public string CompanyLogoUrl { get; set; }
	public Address CompanyAddress { get; set; }
	public Address BillingAddress { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
	public List<LineItem> LineItems { get; set; }
	public decimal TotalPrice => LineItems.Sum(x => x.TotalPrice);
    }

    public class LineItem
    {
	public int Id { get; set; }
	public string ItemName { get; set; }
	public int Quantity { get; set; }
	public decimal PricePerItem { get; set; }
	public decimal TotalPrice => Quantity * PricePerItem;
    }

    public class Address
    {
	public string Name { get; set; }
	public string AddressLine1 { get; set; }
	public string City { get; set; }
	public string PinCode { get; set; }
	public string Country { get; set; }
	public string Email { get; set; }
    }

    public class PaymentMethod
    {
	public string Name { get; set; }
	public string ReferenceNumber { get; set; }
    }
}
