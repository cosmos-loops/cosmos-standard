using System;
using System.Collections.Generic;
using System.Globalization;
using Cosmos.Conversions.Common;
using Cosmos.Conversions.Common.Core;
using Cosmos.Exceptions;

namespace Cosmos.Conversions.Determiners
{
    /// <summary>
    /// Internal core conversion helper from string to sbyte
    /// </summary>
    public static class StringSByteDeterminer
    {
        // ReSharper disable once InconsistentNaming
        internal static bool IS(string str) => Is(str);
        
        /// <summary>
        /// Is
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <param name="formatProvider"></param>
        /// <param name="matchedCallback"></param>
        /// <returns></returns>
        public static bool Is(
            string text, 
            NumberStyles style = NumberStyles.Integer,
            IFormatProvider formatProvider = null, 
            Action<sbyte> matchedCallback = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            var result = sbyte.TryParse(text, style, formatProvider.SafeNumber(), out var number);
            if (!result)
                result = ValueDeterminer.IsXxxAgain<sbyte>(text);
            if (result)
                matchedCallback?.Invoke(number);
            return result;
        }

        /// <summary>
        /// Is
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tries"></param>
        /// <param name="style"></param>
        /// <param name="formatProvider"></param>
        /// <param name="matchedCallback"></param>
        /// <returns></returns>
        public static bool Is(
            string text, 
            IEnumerable<IConversionTry<string, sbyte>> tries,
            NumberStyles style = NumberStyles.Integer, 
            IFormatProvider formatProvider = null, 
            Action<sbyte> matchedCallback = null)
        {
            return ValueDeterminer.IsXXX(text, string.IsNullOrWhiteSpace,
                (s, act) => Is(s, style, formatProvider.SafeNumber(), act), tries, matchedCallback);
        }

        /// <summary>
        /// To
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultVal"></param>
        /// <param name="style"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static sbyte To(
            string text, 
            sbyte defaultVal = default,
            NumberStyles style = NumberStyles.Integer, 
            IFormatProvider formatProvider = null)
        {
            if (sbyte.TryParse(text, style, formatProvider.SafeNumber(), out var number))
                return number;

            return Try.Create(() => Convert.ToSByte(Convert.ToDecimal(text)))
                      .Recover(_ => ValueConverter.ToXxxAgain(text, defaultVal))
                      .Value;
        }

        /// <summary>
        /// To
        /// </summary>
        /// <param name="text"></param>
        /// <param name="impls"></param>
        /// <param name="style"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static sbyte To(
            string text,
            IEnumerable<IConversionImpl<string, sbyte>> impls,
            NumberStyles style = NumberStyles.Integer, 
            IFormatProvider formatProvider = null)
        {
            return ValueConverter.ToXxx(text, (s, act) => Is(s, style, formatProvider.SafeNumber(), act), impls);
        }
    }
}