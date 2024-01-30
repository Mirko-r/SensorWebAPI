using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using WebApi.Models;

namespace WebApplicationTraining.Models
{// popolazione database
    public class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            context.Database.Migrate(); // porto le classi dichiarate nel db creando le tabelle corrispondenti

            if(context.Users.Count() == 0 ) {
                User a = new User
                {
                    Username = "admin",
                    Password = "123",
                    Role = "ADMIN, DEV",
                };

                a.GenerateHash();

                User b = new User
                {
                    Username = "user",
                    Password = "123",
                    Role = "MGR"
                };

                b.GenerateHash();

                context.Users.Add(a);
                context.Users.Add(b);

                context.SaveChanges();
            }
        }
    }
}