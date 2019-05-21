﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.Guava
{
    public partial class Joiner : IGuavaTupleJoiner
    {
        #region SkipValueNulls

        IGuavaTupleJoiner IGuavaTupleJoiner.SkipNulls()
        {
            Options.SetSkipTupleValueNulls();
            return this;
        }

        IGuavaTupleJoiner IGuavaTupleJoiner.SkipNulls(SkipNullType type)
        {
            Options.SetSkipTupleValueNulls(type);
            return this;
        }

        #endregion

        #region UseForNull

        IGuavaTupleJoiner IGuavaTupleJoiner.UseForNull<T1, T2>(Func<T1, T2, T1> tupleKeyFunc, Func<T1, T2, T2> tupleValueFunc)
        {
            Options.SetTupleReplace(tupleKeyFunc, tupleValueFunc);
            return this;
        }

        #endregion

        #region Join - Tuple

        string IGuavaTupleJoiner.Join(IEnumerable<(string, string)> list)
        {
            var replacer = Options.GetTupleReplace<string, string, string, string>();
            var defaultKey = replacer.KeyFunc?.Invoke(string.Empty, string.Empty) ?? string.Empty;
            var defaultValue = replacer.ValueFunc?.Invoke(string.Empty, string.Empty) ?? string.Empty;
            return JoinToTupleString(list, defaultKey, defaultValue, k => k, v => v, replacer);
        }

        string IGuavaTupleJoiner.Join(IEnumerable<(string, string)> list, string defaultKey, string defaultValue)
        {
            var replacer = Options.GetTupleReplace<string, string, string, string>();
            return JoinToTupleString(list, defaultKey, defaultValue, k => k, v => v, replacer);
        }

        string IGuavaTupleJoiner.Join<T1, T2>(IEnumerable<(T1, T2)> list, Func<T1, string> keyFunc, Func<T2, string> valueFunc)
        {
            return JoinToTupleString(list, default, default, keyFunc, valueFunc, Options.GetTupleReplace<T1, T2>());
        }

        string IGuavaTupleJoiner.Join<T1, T2>(IEnumerable<(T1, T2)> list, T1 defaultKey, T2 defaultValue, Func<T1, string> keyFunc, Func<T2, string> valueFunc)
        {
            return JoinToTupleString(list, defaultKey, defaultValue, keyFunc, valueFunc, Options.GetTupleReplace<T1, T2>());
        }

        string IGuavaTupleJoiner.Join((string, string) tuple1, params (string, string)[] restTuples)
        {
            var list = new List<(string, string)> { tuple1 };
            list.AddRange(restTuples);
            return ((IGuavaTupleJoiner)this).Join(list);
        }

        string IGuavaTupleJoiner.Join<T1, T2>(Func<T1, string> keyFunc, Func<T2, string> valueFunc, (T1, T2) tuple1, params (T1, T2)[] restTuples)
        {
            var list = new List<(T1, T2)> { tuple1 };
            list.AddRange(restTuples);
            return ((IGuavaTupleJoiner)this).Join(list, keyFunc, valueFunc);
        }

        private string JoinToTupleString<T1, T2>(IEnumerable<(T1, T2)> list, T1 defaultKey, T2 defaultValue, Func<T1, string> keyFunc, Func<T2, string> valueFunc,
            (Func<T1, T2, T1> KeyFunc, Func<T1, T2, T2> ValueFunc) replacer)
        {
            if (list == null)
                return string.Empty;

            var instances = list.ToList();
            if (!instances.Any())
                return string.Empty;

            var middle = new List<string>();
            foreach (var instance in instances)
            {
                var k = instance.Item1;
                var v = instance.Item2;
                var key = keyFunc(k);
                var value = valueFunc(v);

                if (JoinerUtils.SkipTuple(Options, key, value))
                    continue;
                else if (JoinerUtils.NeedFixTupleValue(Options, key, value))
                {
                    key = JoinerUtils.FixTupleKeySafety(k, key, v, value, defaultKey, keyFunc, replacer.KeyFunc, Options.SkipTupleValueNullType);
                    value = JoinerUtils.FixTupleValueSafety(v, value, k, key, defaultValue, valueFunc, replacer.ValueFunc, Options.SkipTupleValueNullType);
                }

                middle.Add($"{key}{Options.MapSeparator}{value}");
            }

            return middle.JoinToString(_on, JoinerUtils.GetTuplePredicate(Options));
        }

        #endregion

        #region Private class

        private partial class JoinerOptions
        {
            #region Skip Value Nulls - Tuple

            public bool SkipTupleValueNullsFlag { get; private set; }

            public SkipNullType SkipTupleValueNullType { get; private set; } = SkipNullType.Nothing;

            public void SetSkipTupleValueNulls()
            {
                SkipTupleValueNullsFlag = true;
                SkipTupleValueNullType = SkipNullType.WhenBoth;
            }

            public void SetSkipTupleValueNulls(SkipNullType type)
            {
                SkipTupleValueNullsFlag = type != SkipNullType.Nothing;
                SkipTupleValueNullType = type;
            }

            #endregion

            #region UseForNull - Tuple

            private JoinerObjectReplacer TupleReplacer { get; set; }

            private bool TupleValueReplacerFlag { get; set; }

            public (Func<T1, T2, T1> KeyFunc, Func<T1, T2, T2> ValueFunc) GetTupleReplace<T1, T2>()
            {
                return GetTupleReplace<T1, T2, T1, T2>();
            }
            
            public (Func<T1, T2, T1> KeyFunc, Func<T3, T4, T4> ValueFunc) GetTupleReplace<T1, T2, T3, T4>()
            {
                var keyFunc = TupleValueReplacerFlag ? TupleReplacer?.GetTupleKey<T1, T2>() : null;
                var valueFunc = TupleValueReplacerFlag ? TupleReplacer?.GetTupleValue<T3, T4>() : null;
                return (keyFunc, valueFunc);
            }

            public void SetTupleReplace<T1, T2, T3, T4>(Func<T1, T2, T1> tupleKeyFunc, Func<T3, T4, T4> tupleValueFunc)
            {
                TupleValueReplacerFlag = true;
                TupleReplacer = JoinerObjectReplacer.CreateForTuple(tupleKeyFunc, tupleValueFunc);
                SetSkipTupleValueNulls(SkipNullType.Nothing);
            }

            #endregion
        }

        private static partial class JoinerUtils
        {
            public static bool SkipTuple(JoinerOptions options, string key, string value)
            {
                if (options.SkipTupleValueNullsFlag)
                {
                    switch (options.SkipTupleValueNullType)
                    {
                        case SkipNullType.Nothing:
                            return false;

                        case SkipNullType.WhenBoth:
                            return string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value);

                        case SkipNullType.WhenEither:
                            return string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value);

                        case SkipNullType.WhenKeyIsNull:
                            return string.IsNullOrWhiteSpace(key);

                        case SkipNullType.WhenValueIsNull:
                            return string.IsNullOrWhiteSpace(value);
                    }
                }

                return false;
            }

            public static bool NeedFixTupleValue(JoinerOptions options, string key, string value)
            {
                switch (options.SkipTupleValueNullType)
                {
                    case SkipNullType.Nothing:
                        return string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value);

                    case SkipNullType.WhenBoth:
                        return false;

                    case SkipNullType.WhenEither:
                        return false;

                    case SkipNullType.WhenKeyIsNull:
                        return !string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(value);

                    case SkipNullType.WhenValueIsNull:
                        return string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value);
                }

                return false;
            }

            public static string FixTupleKeySafety<T1, T2>(T1 k, string key, T2 v, string value, T1 defaultKey, Func<T1, string> to, Func<T1, T2, T1> keyFunc, SkipNullType type)
            {
                if (!string.IsNullOrWhiteSpace(key))
                    return key;

                if (type == SkipNullType.WhenEither)
                    return key;

                if (type == SkipNullType.Nothing || (type == SkipNullType.WhenValueIsNull && !string.IsNullOrWhiteSpace(value)))
                {
                    return to(keyFunc == null ? defaultKey : keyFunc(k, v));
                }

                return key;
            }

            public static string FixTupleValueSafety<T1, T2>(T2 v, string value, T1 k, string key, T2 defaultValue, Func<T2, string> to, Func<T1, T2, T2> valueFunc, SkipNullType type)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                if (type == SkipNullType.WhenEither)
                    return value;

                if (type == SkipNullType.Nothing || (type == SkipNullType.WhenKeyIsNull && !string.IsNullOrWhiteSpace(key)))
                {
                    return to(valueFunc == null ? defaultValue : valueFunc(k, v));
                }

                return value;
            }

            public static Func<string, bool> GetTuplePredicate(JoinerOptions options)
            {
                if (options.SkipTupleValueNullsFlag)
                    return s => s != options.MapSeparator;
                return null;
            }
        }

        #endregion
    }

    public interface IGuavaTupleJoiner
    {
        IGuavaTupleJoiner SkipNulls();
        IGuavaTupleJoiner SkipNulls(SkipNullType type);
        IGuavaTupleJoiner UseForNull<T1, T2>(Func<T1, T2, T1> tupleKeyFunc, Func<T1, T2, T2> tupleValueFunc);
        string Join(IEnumerable<(string, string)> list);
        string Join(IEnumerable<(string, string)> list, string defaultKey, string defaultValue);
        string Join<T1, T2>(IEnumerable<(T1, T2)> list, Func<T1, string> keyFunc, Func<T2, string> valueFunc);
        string Join<T1, T2>(IEnumerable<(T1, T2)> list, T1 defaultKey, T2 defaultValue, Func<T1, string> keyFunc, Func<T2, string> valueFunc);
        string Join((string, string) tuple1, params (string, string)[] restTuples);
        string Join<T1, T2>(Func<T1, string> keyFunc, Func<T2, string> valueFunc, (T1, T2) tuple1, params (T1, T2)[] restTuples);
    }
}
