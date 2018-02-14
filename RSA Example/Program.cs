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

            KeyGen keys = new KeyGen(64);

            keys.GenerateKeys();
            BigInteger cipher = keys.Encrypt(420);
            BigInteger ms = keys.Decrypt(cipher);
            Console.WriteLine(ms);

            Console.ReadKey();

        }
    }
}
