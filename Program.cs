using System;
using System.Linq;
using System.Threading;

namespace PRNG_PoC_Predictor
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            // create array of random numbers
            int[] randomNumbers = new int[20];
            for (int i = 0; i < randomNumbers.Length; i++)
            {
                randomNumbers[i] = random.Next(1, 10);
            }

            int seed = GuessRandomNumbers(randomNumbers);

            // generate random numbers with the seed
            var dublicated_random = new Random(seed);
            int[] randomNumbers2 = new int[20];
            for (int i = 0; i < randomNumbers2.Length; i++)
            {
                randomNumbers2[i] = dublicated_random.Next(1, 10);
            }
            Console.WriteLine("===================================================");
            Console.WriteLine("New array with same random seed as the first array: ");
            Console.WriteLine("===================================================");
            // compare the two arrays
            for (int i = 0; i < randomNumbers.Length; i++)
            {
                Console.WriteLine("Initial: " + randomNumbers[i] + " Guessed: " + randomNumbers2[i]);
            }
            Console.WriteLine("===================================================");
            Console.WriteLine("Predict next 20 random numbers: ");
            Console.WriteLine("===================================================");
            
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Initial: " + random.Next(1, 10) + " Guessed: " + dublicated_random.Next(1, 10));
            }
        }

        static int GuessRandomNumbers(int[] randomNumbers)
        {
            // create n Threads to guess random numbers
            int threadCount = 6;
            int iterationsPerThread = int.MaxValue / threadCount;
            bool found = false;
            int seed = 0;

            for (int i = 0; i < 6; i++)
            {
                var thread = new Thread(() =>
                {
                    int start = iterationsPerThread * --threadCount;
                    int end = start + iterationsPerThread;
                    int[] rndms = randomNumbers;

                    while(!found){
                        if (start % 1000000 == 0){
                            // print progress in percentage
                            Console.WriteLine("Thead {0} is at {1} %", Thread.CurrentThread.ManagedThreadId, (1- (double)(end - start) / (double)iterationsPerThread) * 100);
                        }
                        // create new random with seed
                        var random = new Random(start);
                        // create array of random numbers
                        int[] randomNumbersGuess = new int[20];
                        for (int i = 0; i < randomNumbersGuess.Length; i++)
                        {
                            randomNumbersGuess[i] = random.Next(1, 10);
                        }
                        // check if arrays are equal
                        if (randomNumbersGuess.SequenceEqual(rndms))
                        {
                            found = true;
                            Console.WriteLine("Found seed: " + start);
                            seed = start;
                            return;
                        }
                        start++;
                        if (start > end)
                        {
                            Console.WriteLine("Thread " + i + " finished");
                            break;
                        }
                    }
                });
                thread.Start();
            }

            // wait for threads to finish
            while (seed == 0)
            {
                Thread.Sleep(100);
            }
            return seed;
        }
    }
}
