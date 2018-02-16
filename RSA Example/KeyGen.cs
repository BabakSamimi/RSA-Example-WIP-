using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;


namespace RSA_Example
{
    class KeyGen
    {
        // Class constructor that takes in the size of the private key (n), otherwise known as the modulus
        public KeyGen(int b)
        {
            bits = b;
            bytes = bits / 8;
        }

        public KeyGen() { }

        public BigInteger q, p, n, m, d, e;

        public int bits;
        private int bytes;

        private void GeneratePrimes()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            byte[] randomValues = new byte[bytes / 2];


            do
            {
                provider.GetBytes(randomValues);
                q = new BigInteger(randomValues);

                q = ReturnPositive(q, randomValues);

            } while (!FermatTest(q, 100));
            do
            {
                provider.GetBytes(randomValues);
                p = new BigInteger(randomValues);

                p = ReturnPositive(p, randomValues);
            } while (!FermatTest(p, 100));

        }

        private BigInteger ReturnPositive(BigInteger temp, byte[] v)
        {
            if (temp < BigInteger.Zero)
            {
                for (int i = 0; i < v.Length; ++i)
                {
                    v[i] ^= 0xFF;
                }

                temp = new BigInteger(v);
                temp += 1;
                return temp;
            }

            return temp;

        }


        private bool FermatTest(BigInteger number, int k)
        {

            if (number == 2 || number == 3) return true;
            if (number < 2 || number % 2 == 0) return false;


            BigInteger e = number - 1;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] tempNumber = number.ToByteArray();
            BigInteger temp;

            for (int i = 0; i < k; ++i)
            {
                do
                {
                    rng.GetBytes(tempNumber);
                    tempNumber[tempNumber.Length - 1] &= 127;
                    temp = new BigInteger(tempNumber);

                } while (temp < 1 || (temp > (number - 1)));

                if (BigInteger.ModPow(temp, e, number) != 1) return false;
            }

            return true;
        }


        public Tuple<BigInteger, BigInteger> Diophantine(BigInteger a, BigInteger b)
        {
            if (b <= 1 || a <= 1 || a % b == 0 || b % a == 0)
            {
                Console.WriteLine("Invalid inputs!");
                return new Tuple<BigInteger, BigInteger>(-1, -1);
            }

            Stack<BigInteger> elements = new Stack<BigInteger>();

            BigInteger temp;
            while ((temp = BigInteger.Remainder(a, b)) != BigInteger.Zero)
            {

                elements.Push((a / b) * BigInteger.MinusOne);
                a = b;
                b = temp;
            }

            Tuple<BigInteger, BigInteger> tuple = new Tuple<BigInteger, BigInteger>(BigInteger.One, elements.Pop());

            while (elements.Count != 0)
            {
                tuple = new Tuple<BigInteger, BigInteger>(tuple.Item2, tuple.Item1 + (tuple.Item2 * elements.Pop()));
            }

            return tuple;
        }

        public void GenerateKeys()
        {

            e = new BigInteger(65537); // Public number

            Console.WriteLine("Encryption initialized.");

            while (true)
            {
                GeneratePrimes();
                n = BigInteger.Multiply(p, q);
                m = BigInteger.Multiply((p - BigInteger.One), (q - BigInteger.One)) / BigInteger.GreatestCommonDivisor(p - 1, q - 1);
                d = Diophantine(m, e).Item2 + m;

                if ((d.Sign != -1) && (BigInteger.GreatestCommonDivisor(m, e) == 1)) break;
            }

            Console.WriteLine("Encryption process completed.");

            //Console.WriteLine("Generated primes...");


            //Console.WriteLine($"Item 1: {Diophantine(m,e).Item1} Item2: {Diophantine(m,e).Item2}");

            //Console.WriteLine($"\n\nQ: {q}\n\n P: {p}\n\n n: {n}\n\nm: {m}\n\n e: {e}\n\n d: {d}");
        }

        public byte[] Encrypt(byte[] rawData)
        {
            return BigInteger.ModPow(new BigInteger(rawData), e, n).ToByteArray();
        }

        public byte[] Decrypt(byte[] cipherData)
        {
            return BigInteger.ModPow(new BigInteger(cipherData), d, n).ToByteArray();
        }

    }
}