using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace WebApplicationTraining.Models
{
    public class User
    {
        public long UserId { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role {  get; set; }
        public string HashToVerify { get; private set; }
        public string SaltToVerify { get; private set; }

        public void GenerateHash()
        {
            var rfc2898 = new Rfc2898DeriveBytes(Password, 30, 310000);
            var hash = Convert.ToBase64String(rfc2898.GetBytes(20));
            var salt = Convert.ToBase64String(rfc2898.Salt);
            this.HashToVerify = hash;
            this.SaltToVerify = salt;
            this.Password = hash + salt;
        }

        public bool VerifyPassword(string passwordToCheck, string hashVer, string saltVer)
        {
            var salt = Convert.FromBase64String(saltVer);
            var rfc2898 = new Rfc2898DeriveBytes(passwordToCheck, salt, 310000);
            var hash = Convert.ToBase64String(rfc2898.GetBytes(20));
            var isValid = hash == hashVer;
            return isValid;
        }

    }
}