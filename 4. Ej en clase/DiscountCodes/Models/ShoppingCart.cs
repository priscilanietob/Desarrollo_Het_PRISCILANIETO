using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscountCodes.Models
{
    public class ShoppingCart
    {
        private readonly List<Product> items = new List<Product>();
        public IReadOnlyList<Product> Items => items.AsReadOnly();

        private string appliedDiscountCode = "";

        public void AddItem(Product product)
        {
            items.Add(product);
        }

        public decimal GetTotal()
        {
            decimal total = items.Sum(item => item.Price);

            switch (appliedDiscountCode)
            {
                case "10PERCENTOFF":
                    total *= 0.9m; 
                    break;

            }

            return total;
        }

        public void ApplyDiscount(string code)
        {
            appliedDiscountCode = code?.ToUpperInvariant() ?? "";
        }
    }
}
