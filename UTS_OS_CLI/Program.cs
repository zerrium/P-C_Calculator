using System;

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
                Console.WriteLine("Masukkan nilai set (n): ");
                input = Console.ReadLine();
                if (!Int32.TryParse(input, out n) || n < 0)
                {
                    Console.WriteLine("Hanya bisa memasukkan bilangan bulat positif");
                    Console.Clear();
                }
                else break;
            }

            while (true)
            {
                Console.WriteLine("Masukkan nilai sub-set (r): ");
                input = Console.ReadLine();
                if (!Int32.TryParse(input, out r) || r < 0)
                {
                    Console.WriteLine("Hanya bisa memasukkan bilangan bulat positif");

                }
                else if (r > n)
                {
                    Console.WriteLine("Nilai sub-set (r) tidak bisa lebih besar dari nilai set (n)");

                }
                else break;

                Console.Clear();
                Console.WriteLine(operasi == 1 ? "Operasi Permutasi" : "Operasi Kombinasi");
                Console.WriteLine("Masukkan nilai set (n): " + n);
            }

            //Mulai ngitung
        }

        private static int Menu()
        {
            Console.Clear();
            x:
            Console.WriteLine("Operasi perhitungan: ");
            Console.WriteLine("1. Permutasi");
            Console.WriteLine("2. Kombinasi");
            Console.WriteLine("3. Keluar\n");
            Console.WriteLine("Pilih: ");
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
    }
}
