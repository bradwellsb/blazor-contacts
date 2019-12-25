using BlazorContacts.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorContacts.API.Data
{
    public class DbInitializer
    {
        public static void Initialize(ContactsContext context)
        {
            //context.Database.EnsureCreated();

            // Look for any Contacts.
            if (context.Contacts.Any())
            {
                return;   // DB has been seeded
            }

            var contacts = new Contact[]
            {
                new Contact { Name = "Person 1",  PhoneNumber = "+1 555 111 1111" },
                new Contact { Name = "Person 2",  PhoneNumber = "+1 555 222 2222" },
                new Contact { Name = "Person 3",  PhoneNumber = "+1 555 333 3333" },
                new Contact { Name = "Person 4",  PhoneNumber = "+1 555 444 4444" },
                new Contact { Name = "Person 5",  PhoneNumber = "+1 555 555 5555" },
            };

            foreach (Contact c in contacts)
            {
                context.Contacts.Add(c);
            }
            context.SaveChanges();
        }
    }
}
