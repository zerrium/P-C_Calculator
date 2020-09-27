using System;
using System.Numerics;
using System.Threading.Tasks;

namespace UTS_OS_CLI
{
    class MainClass
    {
        private static bool debug = false;

        public static void Main(string[] args)
        {
            Console.WriteLine("Permutation and Combination Calculator by Willy Susilo");
            if (args.Length > 0) //take argument
            {
                debug = args[0].ToLower().Equals("debug"); //enables debugging message
                Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Debugging is enabled");
            }

            ulang:
            string input;
            int n, r, operasi;

            do operasi = Menu();
            while (operasi == 0);

            while (true)
            {
                Console.WriteLine(operasi == 1 ? "Operasi Permutasi" : "Operasi Kombinasi");
                Console.Write("Masukkan nilai set (n): ");
                input = Console.ReadLine();
                if (!Int32.TryParse(input, out n) || n < 0)
                {
                    Console.Clear();
                    Console.WriteLine("Hanya bisa memasukkan bilangan bulat positif");
                }
                else break;
            }

            while (true)
            {
                Console.Write("Masukkan nilai sub-set (r): ");
                input = Console.ReadLine();
                if (!Int32.TryParse(input, out r) || r < 0)
                {
                    Console.Clear();
                    Console.WriteLine("Hanya bisa memasukkan bilangan bulat positif");

                }
                else if (r > n)
                {
                    Console.Clear();
                    Console.WriteLine("Nilai sub-set (r) tidak bisa lebih besar dari nilai set (n)");

                }
                else break;

                Console.WriteLine(operasi == 1 ? "Operasi Permutasi" : "Operasi Kombinasi");
                Console.WriteLine("Masukkan nilai set (n): " + n);
            }

            //Mulai ngitung
            Console.WriteLine("\nMenghitung hasil...");

            string result = operasi == 1 ? CountPermutation(n, r).Result.ToString("R") : CountCombination(n, r).Result.ToString("R"); //preserve the whole BigInteger value
            Console.WriteLine("Hasil " + (operasi == 1 ? "permutasi:" : "kombinasi:") + "\n" + result);
            Console.WriteLine("\n\nTekan tombol apapun untuk kembali ke menu utama...");
            Console.ReadLine();
            Console.Clear();
            goto ulang;
        }

        private static int Menu()
        {
            Console.Clear();
            x:
            Console.WriteLine("Operasi perhitungan: ");
            Console.WriteLine("1. Permutasi");
            Console.WriteLine("2. Kombinasi");
            Console.WriteLine("3. Keluar\n");
            Console.Write("Pilih: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    return 1;

                case "2":
                    Console.Clear();
                    return 2;

                case "3":
                    Console.WriteLine("Tekan tombol apapun untuk keluar...");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Pilihan salah. Harap coba lagi");
                    goto x;
            }
            return 0;
        }

        private static async Task<BigInteger> CountPermutation(int n, int r) //formula: n! / (n-r)! running asynchronously from main Thread
        {
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "CountPermutation Task initiated");
            Task<BigInteger>[] tasks = new Task<BigInteger>[2];
            tasks[0] = Task.Run(() => Hitung("atas", "CountPermutation", n));
            tasks[1] = Task.Run(() => Hitung("bawah", "CountPermutation", n - r));

            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Waiting for Task atas and bawah of CountPermutation...");
            await Task.WhenAll(tasks); //wait for all tasks to be done
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "All tasks are done. Result of CountPermutation returned");
            return tasks[0].Result / tasks[1].Result;
        }

        private static async Task<BigInteger> CountCombination(int n, int r) //formula: n! / (r! * (n-r)!) running asynchronously from main Thread
        {
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "CountCombination Task initiated");
            Task<BigInteger>[] tasks = new Task<BigInteger>[3];
            tasks[0] = Task.Run(() => Hitung("atas", "CountCombination", n));
            tasks[1] = Task.Run(() => Hitung("bawah kiri", "CountCombination", r));
            tasks[2] = Task.Run(() => Hitung("bawah kanan", "CountCombination", n - r));

            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Waiting for Task atas, bawah kiri and bawah kanan of CountCombination...");
            await Task.WhenAll(tasks); //wait for all tasks to be done
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "All tasks are done. Result of CountCombination returned");
            return tasks[0].Result / (tasks[1].Result * tasks[2].Result);
        }

        private static async Task<BigInteger> Hitung(string name, string func, int x) //debugging task
        {
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Task " + name + " of " + func + " has started");
            BigInteger temp = Facto(x);
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Task " + name + " of " + func + " has finished");
            return temp;
        }

        private static BigInteger Facto(BigInteger x) //factorial running synchronously
        {
            if (debug) Console.WriteLine("[Debug " + DateTime.Now.ToString("HH:mm:ss.fff] ") + "Task Factorial looping started");
            //return x >= 1 ? x * Facto(x - 1) : 1; // 1!=1 and 0!=1 and stop recursion
            BigInteger temp = 1;
            for (BigInteger i = x; i > 0; i--) //can't use recursion due to stackoverflowException when entering big numbers
            {
                temp *= i;
            }
            return temp;
        }
    }
}
