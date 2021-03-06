﻿namespace Cosmos
{
    public static class BooleanVal
    {
        public static BooleanVal<T> Of<T>(bool value, T obj) => BooleanVal<T>.Of(value, obj);
    }
    
    /// <summary>
    /// Boolean value wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BooleanVal<T>
    {
        public BooleanVal(bool value, T obj)
        {
            Value = value;
            Object = obj;
        }

        public bool Value { get; }

        public T Object { get; }

        public static implicit operator (bool Value, T Object)(BooleanVal<T> val)
        {
            return (val.Value, val.Object);
        }

        public static implicit operator BooleanVal<T>((bool Value, T Object) tuple)
        {
            return new(tuple.Value, tuple.Object);
        }

        public static implicit operator bool(BooleanVal<T> val)
        {
            return val.Value;
        }

        public static implicit operator BooleanVal<T>(bool value)
        {
            return new(value, default);
        }

        public static BooleanVal<T> Of(bool value, T obj) => new(value, obj);
    }

    public static class BooleanValExtensions
    {
        public static BooleanVal<T> ToBooleanVal<T>(this bool value) => value;

        public static BooleanVal<T> ToBooleanVal<T>(this bool value, T packagedValue) => new(value, packagedValue);

        public static BooleanVal<T> ToBooleanVal<T>(this (bool, T) tuple) => tuple;
    }
}