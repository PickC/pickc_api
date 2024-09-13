using System.Security.Cryptography;
using System.Text;
namespace appify.utility
{
    public static class DataHash
    {
        private static readonly string SecretKey = "App1fyd3v3l0p3r";
        public static string EncryptData(string input)
        {
            byte[] RandomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(RandomNumber);
            }

            string SecretPassword = input + Convert.ToBase64String(RandomNumber) + SecretKey;
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(SecretPassword));
                byte[] hashbytes = new byte[hash.Length + RandomNumber.Length];
                Array.Copy(RandomNumber, 0, hashbytes, 0, RandomNumber.Length);
                Array.Copy(hash, 0, hashbytes, RandomNumber.Length, hash.Length);
                return Convert.ToBase64String(hashbytes);
            }
        }

        public static    bool DecryptData(string input, string storedHash)
        {
            byte[] hashbytes  = Convert.FromBase64String(storedHash);
            byte[] RandomNumber = new byte[32];

            Array.Copy(hashbytes,0, RandomNumber, 0,RandomNumber.Length);
            string SecretPassword = input + Convert.ToBase64String(RandomNumber) + SecretKey;

            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(SecretPassword));

                for(int i=0;i<hash.Length; i++)
                {
                    if (hashbytes[i + RandomNumber.Length]!=hash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
