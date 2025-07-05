// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace XazeAPI.API.Extensions
{
    public static class DictionaryExtensions
    {
        public static TSource RandomItem<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate = null)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (source.Count() == 0)
            {
                return default;
            }

            if (source.Count() == 1)
            {
                return source.FirstOrDefault();
            }

            if (predicate != null)
            {
                return source.Where(predicate).ElementAt(Random.Range(0, source.Where(predicate).Count()));
            }

            return source.ElementAt(Random.Range(0, source.Count()));
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException("source");

            foreach (var element in source)
            {
                action(element);
            }
        }

        public static bool TryGetFirst<T>(
                this IEnumerable<T> source,
                Func<T, bool> predicate,
                out T result)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result = item;
                    return true;
                }
            }

            result = default!;
            return false;
        }

    }
}
