using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageConverterLib;

namespace BinToImg
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter row size: ");
            string rowString = Console.ReadLine();
            int rowWidth;
            try
            {
                rowWidth = int.Parse(rowString);
            }
            catch
            {
                Console.WriteLine("Wrong data");
                Console.ReadKey(true);
                return;
            }

            Console.Write("Source file: ");
            byte[] data;
            try
            {
                data = File.ReadAllBytes(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("File does not exist");
                Console.ReadKey(true);
                return;
            }

            Bitmap res = ImageEncoder.Encode(data, rowWidth);

            Console.Write("Result file: ");
            try
            {
                res.Save(Console.ReadLine(), ImageFormat.Png);
            }
            catch
            {
                Console.WriteLine("Cannot write");
                Console.ReadKey(true);
                return;
            }
        }
    }
}
