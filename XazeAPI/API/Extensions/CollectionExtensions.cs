// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace XazeAPI.API.Extensions;

// This is taken from the Lib.Harmony NuGet Package
// HarmonyLib.Extensions | 0Harmony.dll
// Credit goes to the Original Creators
public static class CollectionExtensions
{
    /// <param name="sequence">The collection</param>
    /// <typeparam name="T">The inner type of the collection</typeparam>
    extension<T>(IEnumerable<T> sequence)
    {
        /// <summary>A simple way to execute code for every element in a collection</summary>
        /// <param name="action">The action to execute</param>
        /// 
        public void Do(Action<T> action)
        {
            if (sequence is null) return;
            using var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext()) action(enumerator.Current);
        }

        /// <summary>A simple way to execute code for elements in a collection matching a condition</summary>
        /// <param name="condition">The predicate</param>
        /// <param name="action">The action to execute</param>
        /// 
        public void DoIf(Func<T, bool> condition, Action<T> action) => sequence.Where(condition).Do(action);

        /// <summary>A helper to add an item to a collection</summary>
        /// <param name="item">The item to add</param>
        /// <returns>The collection containing the item</returns>
        /// 
        [SuppressMessage("Style", "IDE0300")]
        public IEnumerable<T> AddItem(T item) => (sequence ?? []).Concat(new T[] { item });
    }

    /// <param name="sequence">The array</param>
    /// <typeparam name="T">The inner type of the collection</typeparam>
    extension<T>(T[] sequence)
    {
        /// <summary>A helper to add an item to an array</summary>
        /// <param name="item">The item to add</param>
        /// <returns>The array containing the item</returns>
        /// 
        public T[] AddToArray(T item) => sequence.AddItem(item).ToArray();

        /// <summary>A helper to add items to an array</summary>
        /// <param name="items">The items to add</param>
        /// <returns>The array containing the items</returns>
        /// 
        public T[] AddRangeToArray(T[] items) => (sequence ?? Enumerable.Empty<T>()).Concat(items).ToArray();
    }
}