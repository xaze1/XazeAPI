// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;
using XazeAPI.Features;

namespace XazeAPI.API.Extensions;

public static class DelegateExtensions
{
    public static void InvokeSafely(this Action action, Action<Exception> onException = null)
    {
        if (action == null) 
            return;

        foreach (var del in action.GetInvocationList())
        {
            if (del is not Action handler) 
                continue;
            try
            {
                handler();
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
        }
    }
    
    public static void InvokeSafely<T>(this Action<T> action, T arg, Action<Exception> onException = null)
    {
        if (action == null)
            return;

        foreach (var del in action.GetInvocationList())
        {
            if (del is not Action<T> handler) 
                continue;
            try
            {
                handler(arg);
            }
            catch (Exception ex)
            {
                if (onException == null)
                    Logging.Error(ex);
                else
                    onException.Invoke(ex);
            }
        }
    }
    
    public static void InvokeSafely<A1, A2>(this Action<A1, A2> action, A1 arg, A2 arg2, Action<Exception> onException = null)
    {
        if (action == null)
            return;

        foreach (var del in action.GetInvocationList())
        {
            if (del is not Action<A1, A2> handler) 
                continue;
            try
            {
                handler(arg, arg2);
            }
            catch (Exception ex)
            {
                if (onException == null)
                    Logging.Error(ex);
                else
                    onException.Invoke(ex);
            }
        }
    }
    
    public static TResult InvokeSafely<TResult>(this Func<TResult> func, Action<Exception> onException = null)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
            return default;
        }
    }
    
    public static TResult InvokeSafely<A1, TResult>(this Func<A1, TResult> func, A1 arg, Action<Exception> onException = null)
    {
        try
        {
            return func.Invoke(arg);
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
            return default;
        }
    }
}