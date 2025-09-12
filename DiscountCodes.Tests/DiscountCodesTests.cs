using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscountCodes.Models
{
    public class ShoppingCart
    {
        private List<Product> _items = new List<Product>();
        private string _appliedDiscountCode = "";

        public void AddItem(Product product)
        {
            _items.Add(product);
        }

        public void ApplyDiscount(string discountCode)
        {
            _appliedDiscountCode = discountCode?.ToUpper() ?? "";
        }

        public decimal GetTotal()
        {
            if (string.IsNullOrEmpty(_appliedDiscountCode))
                return _items.Sum(p => p.Price);

            return _appliedDiscountCode switch
            {
                "BOGOFREE" => CalculateBogoFreeDiscount(),
                "BRAND2DISCOUNT" => CalculateBrand2Discount(),
                "10PERCENTOFF" => CalculateTenPercentOff(),
                "BRAND1MANIA" => CalculateBrand1Mania(),
                "5USDOFF" => CalculateFiveDollarsOff(),
                _ => _items.Sum(p => p.Price) // Código inválido o desconocido
            };
        }

        private decimal CalculateBogoFreeDiscount()
        {
            decimal total = 0;
            var groupedProducts = _items.GroupBy(p => new { p.Brand, p.Name });

            foreach (var group in groupedProducts)
            {
                int count = group.Count();
                int freeItems = count / 2;
                int paidItems = count - freeItems;
                
                total += paidItems * group.First().Price;
            }

            return total;
        }

        private decimal CalculateBrand2Discount()
        {
            decimal total = 0;
            
            foreach (var product in _items)
            {
                if (product.Brand.Equals("Brand2", StringComparison.OrdinalIgnoreCase))
                {
                    total += product.Price * 0.9m; // 10% de descuento
                }
                else
                {
                    total += product.Price;
                }
            }

            return total;
        }

        private decimal CalculateTenPercentOff()
        {
            decimal subtotal = _items.Sum(p => p.Price);
            return subtotal * 0.9m; // 10% de descuento en todo
        }

        private decimal CalculateBrand1Mania()
        {
            decimal total = 0;
            
            foreach (var product in _items)
            {
                if (product.Brand.Equals("Brand1", StringComparison.OrdinalIgnoreCase))
                {
                    total += product.Price * 0.5m; // 50% de descuento
                }
                else
                {
                    total += product.Price;
                }
            }

            return total;
        }

        private decimal CalculateFiveDollarsOff()
        {
            decimal subtotal = _items.Sum(p => p.Price);
            return Math.Max(0, subtotal - 5.00m);
        }
    }
}