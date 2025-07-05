// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Reflection;
using LabApi.Events.CustomHandlers;

namespace XazeAPI.API.Events.Handler;

public static class XazeHandlerManager
{
    public static void RegisterEventsHandler<T>(T handler) where T : XazeEventHandler
    {
      CustomHandlersManager.RegisterEventsHandler(handler);
      Type type = handler.GetType();
      RegisterEvents(handler, type);
    }

    public static void UnregisterEventsHandler<T>(T handler) where T : XazeEventHandler
    {
      foreach (var internalEvent in handler.InternalEvents)
        internalEvent.Key.RemoveEventHandler((object) null, internalEvent.Value);
      handler.InternalEvents.Clear();
    }

    public static void CheckEvent<T>(
      T handler,
      Type handlerType,
      string methodDelegate,
      Type eventType,
      string eventName)
      where T : XazeEventHandler
    {
      MethodInfo method = handlerType.GetMethod(methodDelegate, BindingFlags.Instance | BindingFlags.Public);
      if (method == null || !IsOverride(method))
          return;
      
      EventInfo key = eventType.GetEvent(eventName);
      Delegate handler1 = Delegate.CreateDelegate(key.EventHandlerType, handler, method);
      key.AddEventHandler(null, handler1);
      handler.InternalEvents.Add(key, handler1);
    }

    public static bool IsOverride(MethodInfo method)
    {
      return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
    }

    public static void RegisterEvents<T>(T handler, Type handlerType) where T : XazeEventHandler
    {
        CheckEvent(handler, handlerType, nameof(XazeEventHandler.OnPlayerHearingFakePlayer), typeof(XazeEvents), nameof(XazeEvents.HearingFake));
    }
}