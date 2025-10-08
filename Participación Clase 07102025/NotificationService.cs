using System;
using OrderNotifications.Models;

namespace OrderNotifications
{
    public class NotificationService : INotificationService
    {
        public void NotifyOrderStatus(Order order)
        {
            var customer = order.Customer;

            Console.WriteLine($"\nProcesando notificación para {customer.Name} ({customer.CountryCode})...");

            bool email = true;                              
            bool sms = customer.CountryCode != "MX";         
            bool whatsapp = customer.CountryCode == "MX";    

            if (customer.Preferences != null)
            {
                if (customer.Preferences.ContainsKey(NotificationChannel.Email))
                    email = customer.Preferences[NotificationChannel.Email] == NotificationChannelPreference.Enabled;

                if (customer.Preferences.ContainsKey(NotificationChannel.SMS))
                    sms = customer.Preferences[NotificationChannel.SMS] == NotificationChannelPreference.Enabled;

                if (customer.Preferences.ContainsKey(NotificationChannel.WhatsApp))
                    whatsapp = customer.Preferences[NotificationChannel.WhatsApp] == NotificationChannelPreference.Enabled;
            }

            if (customer.CountryCode == "MX")
            {
                email = true;
                sms = false;
                whatsapp = true;
            }
            else if (customer.CountryCode == "AK")
            {
                email = false;
                sms = false;
                whatsapp = false;
            }

            if (email)
                Console.WriteLine($"Enviando email a {customer.ContactInfo.Email}: Tu orden {order.Id} está {order.Status}");
            if (sms)
                Console.WriteLine($"Enviando SMS a {customer.ContactInfo.PhoneNumber}: Tu orden {order.Id} está {order.Status}");
            if (whatsapp)
                Console.WriteLine($"Enviando WhatsApp a {customer.ContactInfo.PhoneNumber}: Tu orden {order.Id} está {order.Status}");

            if (!email && !sms && !whatsapp)
                Console.WriteLine($"Ningún canal disponible para {customer.Name}");
        }
    }
}
