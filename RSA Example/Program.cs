using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace RSA_Example
{
    class Program
    {

        static void Main(string[] args)
        {


            KeyGen keys = new KeyGen(2048);

            keys.GenerateKeys();

            while (true)
            {
                byte[] msg = Encoding.UTF8.GetBytes(Console.ReadLine());

                byte[] cipher = keys.Encrypt(msg);
                byte[] decipher = keys.Decrypt(cipher);
                string decipher_string = Encoding.UTF8.GetString(decipher);
                Console.WriteLine("\nThe message was: " + decipher_string);

            }


        }
    }
}
