using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Converters
{
    internal static class Password
    {

        internal static List<byte> Encode(string pwd)
        {
            var list = new List<byte>();
            var pwdBytes = Encoding.UTF8.GetBytes(pwd);
            list.AddRange(BitConverter.GetBytes((short)(pwdBytes.Length + 365)));

            foreach (var item in pwdBytes)
            {
                list.Add(item);
                list.Add((byte)Random.Shared.Next(255));
                list.Add((byte)Random.Shared.Next(255));
                list.Add((byte)Random.Shared.Next(255));
                list.Add((byte)Random.Shared.Next(255));
                list.Add((byte)Random.Shared.Next(255));
                list.Add((byte)Random.Shared.Next(255));
            }
            int need = 1024 - list.Count;
            if (need > 0)
            {
                for (int i = 0; i < need; i++)
                {
                    list.Add((byte)Random.Shared.Next(255));
                }
            }

            return list;
        }

        internal static string Parse(byte[] raw)
        {
            var total = BitConverter.ToInt16(raw, 0) - 365;
            var list = new List<byte>();
            var subArray = raw[2..];
            var current = 0;
            for (int i = 0; i < subArray.Length; i++)
            {
                if (i % 7 == 0)
                {
                    current++;
                    list.Add(subArray[i]);
                    if (current == total)
                    {
                        break;
                    }
                }
            }

            var result = Encoding.UTF8.GetString(list.ToArray());

            return result;
        }
    }
}
