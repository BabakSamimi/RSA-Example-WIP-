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

        public KeyGen(int b)
        {
            bits = b;

        }

        public KeyGen() { }

        public BigInteger q, p, n, m, d, e;
        
        public int bits;

        private void GeneratePrimes()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            byte[] randomValues = new byte[(bits/8)/2];


            do
            {
                provider.GetBytes(randomValues);
                q = new BigInteger(randomValues);

                q = ReturnPositive(q, randomValues);

            } while (!FermatTest(q, 200));
            Console.WriteLine("Generated q");
            do
            {
                provider.GetBytes(randomValues);
                p = new BigInteger(randomValues);

                p = ReturnPositive(p, randomValues);
            } while (!FermatTest(p, 200) && (p == q));

            /*do
            {
                provider.GetBytes(randomValues);
                q = new BigInteger(randomValues);

                q = ReturnPositive(q, randomValues);
                
            } while (!MillerRabin(q, 100));
            Console.WriteLine("Generated q");
            do
            {
                provider.GetBytes(randomValues);
                p = new BigInteger(randomValues);

                p = ReturnPositive(p, randomValues);
            } while (!MillerRabin(p, 100) && (p == q));
            */
        }

        private BigInteger ReturnPositive(BigInteger temp, byte[] v)
        {
            if(temp < BigInteger.Zero)
            {
                for(int i = 0; i < v.Length; ++i)
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

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] tempNumber = new byte[(bits / 8) / 2];
            BigInteger temp;

            for (int i = 0; i < k; ++i)
            {
                do
                {
                    rng.GetBytes(tempNumber);
                    temp = new BigInteger(tempNumber);
                    temp = ReturnPositive(temp, tempNumber);

                } while (!(temp > 2 && (temp < (number - 2))) );

                if (BigInteger.ModPow(temp, number - BigInteger.One, number) != BigInteger.One) return false;
            }

            return true;
        }

        private bool MillerRabin(BigInteger number, int k)
        {
            if (number == 2 || number == 3) // 2,3 are primes
                return true;
            if (number < 2 || number % 2 == 0) // anything below 2 and is even returns false because it's not a prime
                return false;


            // Convert n - 1 into 2^(r) * d format
            BigInteger d = number - 1;
            int r = 0;

            while (d % 2 == 0)
            {
                d >>= 1;
                r++;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] temp = new byte[number.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < k; ++i) // Check if composite
            {
                do
                {
                    rng.GetBytes(temp);
                    a = new BigInteger(temp);
                    a = ReturnPositive(a, temp);

                } while (a < 2 || (a >= number - 2));


                BigInteger x = BigInteger.ModPow(a, d, number);

                if (x == 1 || (x == number - 1))
                    continue;

                for(int z = 1; z < r; ++z)
                {
                    x = BigInteger.ModPow(x, 2, number);
                    if (x == 1) return false;
                    if (x == (number - 1))
                        break;
                }


                if (x != (number - 1)) return false; 

            }

            return true; // Probably prime

        }
        

        public Tuple<BigInteger,BigInteger> Diophantine(BigInteger a, BigInteger b)
        {
            if(b <= 1 || a <= 1 || a%b == 0 || b%a == 0)
            {
                Console.WriteLine("Invalid inputs!");
                return new Tuple<BigInteger, BigInteger>(-1, -1);
            }

            Stack<BigInteger> elements = new Stack<BigInteger>();

            BigInteger temp;
            while ((temp = BigInteger.Remainder(a, b))!= BigInteger.Zero)
            {

                elements.Push((a/b) * BigInteger.MinusOne);
                a = b;
                b = temp;
            }

            Tuple<BigInteger, BigInteger> tuple = new Tuple<BigInteger, BigInteger>(BigInteger.One, elements.Pop());
            
            while(elements.Count != 0)
            {
                tuple = new Tuple<BigInteger, BigInteger>(tuple.Item2, tuple.Item1 + (tuple.Item2 * elements.Pop()));
            }

            return tuple;
        }

        public void GenerateKeys()
        {

            e = new BigInteger(65537);

            while (true)
            {
                Console.WriteLine("Generating primes...");
                GeneratePrimes();
                n = BigInteger.Multiply(p, q);
                m = BigInteger.Multiply((p - BigInteger.One), (q - BigInteger.One))/BigInteger.GreatestCommonDivisor(p-1, q-1);
                d = Diophantine(m, e).Item2 + m;

                if ((d.Sign != -1) && (BigInteger.GreatestCommonDivisor(m,e) == 1)) break;
            }
    
            Console.WriteLine("Generated primes...");


            Console.WriteLine($"Item 1: {Diophantine(m,e).Item1} Item2: {Diophantine(m,e).Item2}");

            Console.WriteLine($"\n\nQ: {q}\n\n P: {p}\n\n n: {n}\n\nm: {m}\n\n e: {e}\n\n d: {d}");
        }


        private bool GcdOne(BigInteger left, BigInteger right)
        {
            if (right == BigInteger.Zero) return false;

            if (BigInteger.Remainder(left,right) == BigInteger.One) return true;

            BigInteger rest = new BigInteger();

            do
            {
                rest = BigInteger.Remainder(left, right);
                //rest = left % right;
                left = right;
                right = rest;

                if (rest == BigInteger.One) return true;

            } while (rest != BigInteger.Zero);

            return (rest == BigInteger.One) ? true : false;

        }

        public BigInteger Encrypt(BigInteger number)
        {
            //byte[] rawtext = Encoding.UTF8.GetBytes(msg);


            BigInteger y = BigInteger.ModPow(number, e, n);
            Console.WriteLine("Generated cipher: " + y);

            return y;

        }

        public BigInteger Decrypt(BigInteger cipher)
        {

            BigInteger x = BigInteger.ModPow(cipher, d, n);

            return x;

        }

    }
}