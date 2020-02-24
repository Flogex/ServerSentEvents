using System;

namespace ServerSentEvents
{
    internal static class EncodingHelper
    {
        public static byte[] ToByteArray(this int number)
        {
            if (number < 0)
                throw new ArgumentException("Argument must be greater or equal zero",
                                            nameof(number));

            var digitsCount = GetNumberOfDigits(number);
            var bytes = new byte[digitsCount];

            for (var i = digitsCount - 1; i >= 0; i--)
            {
                var rest = number % 10;
                number /= 10; // Rest is truncated
                bytes[i] = (byte)(rest + 48);
            }

            return bytes;
        }

        private static int GetNumberOfDigits(int number)
        {
            var digitsCount = 1;
            var modulo = 10;

            while (number > modulo - 1)
            {
                digitsCount++;
                modulo *= 10;
            }

            return digitsCount;
        }
    }
}
