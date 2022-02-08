using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework
{
    public static class CollectionUtil
    {
        public static TValue GetValue<TKey,TValue>(this Dictionary<TKey,TValue> dic, TKey key) where TValue : class
        {
            TValue result = null;
            dic.TryGetValue(key, out result);
            return result;
        }

        public static void AddUnique<TValue>(this List<TValue> list, TValue value)
        {
            if (list.Contains(value))
                return;

            list.Add(value);
        }
    }
}