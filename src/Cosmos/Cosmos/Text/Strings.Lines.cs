﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.Text
{
    /// <summary>
    /// String Utils<br />
    /// 字符串工具
    /// </summary>
    public static partial class Strings
    {
        /// <summary>
        /// New line
        /// </summary>
        public const string NEWLINE = "\r\n";
        
        /// <summary>
        /// Count by lines
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int CountByLines(string text)
        {
            int index = 0, lines = 0;

            while (true)
            {
                var newIndex = text.IndexOf(Environment.NewLine, index, StringComparison.Ordinal);
                
                if (newIndex < 0)
                {
                    if (text.Length > index)
                        lines++;

                    return lines;
                }

                index = newIndex + 2;
                lines++;
            }
        }

        /// <summary>
        /// Split by lines
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitByLines(string text)
        {
            var index = 0;

            while (true)
            {
                var newIndex = text.IndexOf(Environment.NewLine, index, StringComparison.Ordinal);
                if (newIndex < 0)
                {
                    if (text.Length > index)
                        yield return text.Substring(index);

                    yield break;
                }

                var currentString = text.Substring(index, newIndex - index);
                index = newIndex + 2;

                yield return currentString;
            }
        }

        /// <summary>
        /// Truncate by lines
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLines"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public static string TruncateByLines(string text, int maxLines, string placeholder = "...")
        {
            if (string.IsNullOrEmpty(text) || maxLines <= 0)
                return string.Empty;
            if (string.IsNullOrEmpty(placeholder))
                placeholder = "...";
            var builder = new StringBuilder();
            var counter = 0;
            foreach(var line in SplitByLines(text))
            {
                if (++counter == maxLines)
                {
                    builder.AppendLine(placeholder);
                    break;
                }

                builder.AppendLine(line);
            }

            return builder.ToString();
        }
    }

    public static partial class StringsExtensions
    {
        /// <summary>
        /// Line count
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int CountByLines(this string text)
        {
            return Strings.CountByLines(text);
        }

        /// <summary>
        /// To lines
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitByLines(this string text)
        {
            return Strings.SplitByLines(text);
        }

        /// <summary>
        /// Truncate by lines
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLines"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public static string TruncateByLines(this string text, int maxLines, string placeholder = "...")
        {
            return Strings.TruncateByLines(text, maxLines, placeholder);
        }
    }
}