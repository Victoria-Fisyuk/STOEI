using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TAiMStore.Domain;
using TAiMStore.Model.ViewModels;

namespace TAiMStore.Model.Repository
{
    public class EmailSettings
    {
        public string MailtoAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = true;
        public string FileLocation = @"~/AppData/sports_store_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor()
        {
            this.emailSettings = new EmailSettings();
        }

        public void ProcessOrder(IEnumerable<CartLine> carts, Order shippingDetails)
        {
            using (var smtpClient = new SmtpClient
            {
                EnableSsl = emailSettings.UseSsl,
                Host = emailSettings.ServerName,
                Port = emailSettings.ServerPort,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
            })
            {
                if (emailSettings.WriteAsFile)
                {
                    string str = HttpContext.Current.Server.MapPath(emailSettings.FileLocation);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = HttpContext.Current.Server.MapPath(emailSettings.FileLocation);
                    smtpClient.EnableSsl = false;
                }

                var body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var line in carts)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", shippingDetails.TotalCost)
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.User.Name)
                    .AppendLine(shippingDetails.User.Contacts.PersonFullName)
                    .AppendLine(shippingDetails.User.Contacts.Organization ?? "")
                    .AppendLine(shippingDetails.User.Contacts.City ?? "")
                    .AppendLine(shippingDetails.User.Contacts.Street ?? "")
                    .AppendLine(shippingDetails.User.Contacts.House ?? "")
                    .AppendLine(shippingDetails.User.Contacts.Room  ?? "")
                    .AppendLine(shippingDetails.User.Contacts.PostZip ?? "")
                    .AppendLine(shippingDetails.User.Contacts.Telephone ?? "")
                    .AppendLine("---");

                var mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailtoAddress, "New order submitted!", body.ToString());

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }


                smtpClient.Send(mailMessage);
            }
        }
    }
}
