using ImageConverterLib;
using System;
using System.Drawing;
using System.IO;

namespace ImgToBin
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
                Console.ReadKey();
                return;
            }

            Console.Write("Source file: ");
            Bitmap bmp;
            try
            {
                 bmp = (Bitmap)Image.FromFile(Console.ReadLine(), false);
            }
            catch
            {
                Console.WriteLine("File does no exist or wrong data");
                Console.ReadKey();
                return;
            }

            byte[] res = ImageEncoder.Decode(bmp, rowWidth);

            Console.Write("Result file: ");
            try
            {
                File.WriteAllBytes(Console.ReadLine(), res);
            }
            catch
            {
                Console.WriteLine("Cannot write");
                Console.ReadKey();
                return;
            }
        }
    }
}
