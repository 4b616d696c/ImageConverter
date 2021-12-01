using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageConverterLib
{
    public static class ImageEncoder
    {
        public static Bitmap Encode(byte[] data, int rowWidth)
        {
            uint len = (uint)data.Length;

            List<Color[]> rows = new List<Color[]>();

            rows.Add(new Color[] { });
            rows.AddRange(CreateRows(data, rowWidth));
            rows[0] = GenerateHeader(len, rowWidth, rows.Count - 1);

            return RowsToBitmap(rows, rowWidth);
        }

        public static byte[] Decode(Bitmap bmp, int rowWidth)
        {
            ValueTuple<List<Color[]>, uint> x = BitmapToRows(bmp, rowWidth);
            byte[] data = CreateData(x.Item1, rowWidth, (int)x.Item2);

            return data;
        }

        private static Color[] GenerateHeader(uint len, int rowWidth, int rows)
        {
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] rowsBytes = BitConverter.GetBytes(rows);
            Color[] header = new Color[rowWidth];

            for (int i = 0; i < rowWidth; i++)
                header[i] = Color.FromArgb(0, 0, 0, 0);

            header[0] = BytesToColor(lenBytes);
            header[1] = BytesToColor(rowsBytes);

            return header;
        }

        private static List<Color[]> CreateRows(byte[] data, int rowWidth)
        {
            List<byte> dataList = new List<byte>(data);
            int bpr = rowWidth * 4;

            if (dataList.Count % bpr != 0)
                for (int i = 0; i < dataList.Count % bpr; i++)
                    dataList.Add(0);

            List<List<byte>> byteRows = new List<List<byte>>();

            List<byte> byteRow = new List<byte>();
            foreach (byte b in dataList)
            {
                byteRow.Add(b);
                if (byteRow.Count >= bpr)
                {
                    byteRows.Add(new List<byte>(byteRow));
                    byteRow.Clear();
                }
            }

            List<Color[]> rows = new List<Color[]>();

            foreach (List<byte> r in byteRows)
            {
                List<Color> row = new List<Color>();
                for (int i = 0; i < bpr; i += 4)
                {
                    row.Add(BytesToColor(r.GetRange(i, 4).ToArray()));
                }
                rows.Add(new List<Color>(row).ToArray());
            }

            return rows;
        }

        private static Bitmap RowsToBitmap(List<Color[]> rows, int rowWidth)
        {
            Bitmap res = new Bitmap(rowWidth, rows.Count);

            for (int y = 0; y < rows.Count; y++)
                for (int x = 0; x < rowWidth; x++)
                    res.SetPixel(x, y, rows[y][x]);

            return res;
        }

        private static ValueTuple<List<Color[]>, uint> BitmapToRows(Bitmap bmp, int rowWidth)
        {
            Color[] header = ReadHeader(bmp, rowWidth);
            uint len = BitConverter.ToUInt32(ColorToBytes(header[0]));
            int rowsCount = BitConverter.ToInt32(ColorToBytes(header[1]));

            List<Color[]> rows = new List<Color[]>();

            for (int y = 1; y < rowsCount + 1; y++)
            {
                List<Color> row = new List<Color>();
                for (int x = 0; x < rowWidth; x++)
                {
                    row.Add(bmp.GetPixel(x, y));
                }
                rows.Add(row.ToArray());
            }

            return new(rows, len);
        }

        private static byte[] CreateData(List<Color[]> rows, int rowWidth, int len)
        {
            List<byte[]> byteRows = new List<byte[]>();
            foreach (Color[] row in rows)
            {
                List<byte> byteRow = new List<byte>();
                foreach (Color c in row)
                    byteRow.AddRange(ColorToBytes(c));
                byteRows.Add(byteRow.ToArray());
            }

            List<byte> data = new List<byte>();
            foreach (byte[] byteRow in byteRows)
                data.AddRange(byteRow);

            data = data.GetRange(0, len);

            return data.ToArray();
        }

        private static Color[] ReadHeader(Bitmap bmp, int headerWidth)
        {
            List<Color> res = new List<Color>();
            for (int i = 0; i < headerWidth; i++)
                res.Add(bmp.GetPixel(i, 0));
            return res.ToArray();
        }

        private static byte[] ColorToBytes(Color c) => new byte[] { c.A, c.R, c.G, c.B };
        private static Color BytesToColor(byte[] b) => Color.FromArgb(b[0], b[1], b[2], b[3]);
    }
}
