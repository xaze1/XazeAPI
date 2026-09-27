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
            if (source == null) throw new ArgumentNullException(nameof(source));

            var sourceList = source.ToList();
            switch (sourceList.Count)
            {
                case 0:
                    return default;
                case 1:
                    return sourceList.FirstOrDefault();
            }

            if (predicate == null) 
                return sourceList.ElementAt(Random.Range(0, sourceList.Count));
            
            var predicateOutcome = sourceList.Where(predicate).ToList();
            if (predicateOutcome.Count == 0)
                return default;
                
            return predicateOutcome.ElementAt(Random.Range(0, predicateOutcome.Count));
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
                action(element);
        }

        public static bool TryGetFirst<T>(
                this IEnumerable<T> source,
                Func<T, bool> predicate,
                out T result)
        {
            foreach (var item in source)
            {
                if (!predicate(item)) 
                    continue;
                result = item;
                return true;
            }

            result = default!;
            return false;
        }
        
        public static IList<T> Shuffle<T>(this IList<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1); // 0 ≤ j ≤ i
                (list[i], list[j]) = (list[j], list[i]);
            }
            
            return list;
        }
    }
}
