// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System;
using System.Collections.Generic;
using System.Reflection;
using LabApi.Events;
using LabApi.Events.CustomHandlers;

namespace XazeAPI.API.Events.Handler;

public static class XazeHandlerManager
{
    public static bool Initialized { get; private set; }
    public static event LabEventHandler RegisterPluginEvents;
    public static event LabEventHandler RegisterEventHandlers;
    
    private struct PluginEvent(string method, EventInfo @event)
    {
        public readonly string methodDelegate = method;
        public readonly EventInfo Event = @event;
    }

    private static readonly List<PluginEvent> Events = new();
  
    public static void RegisterEventsHandler<T>(T handler) where T : XazeEventHandler
    {
      CustomHandlersManager.RegisterEventsHandler(handler);
      Type type = handler.GetType();
      RegisterEvents(handler, type);
    }

    public static void UnregisterEventsHandler<T>(T handler) where T : XazeEventHandler
    {
        foreach (var internalEvent in handler.InternalEvents) 
            internalEvent.Key.RemoveEventHandler(null, internalEvent.Value);
        
        handler.InternalEvents.Clear();
    }

    public static void AddEvent(string method, Type type, string name)
    {
        Events.Add(new PluginEvent(method, type.GetEvent(name)));
    }

    public static void CheckEvent<T>(
      T handler,
      Type handlerType,
      string methodDelegate,
      EventInfo key)
      where T : XazeEventHandler
    {
        MethodInfo method = handlerType.GetMethod(methodDelegate, BindingFlags.Instance | BindingFlags.Public);
        if (method == null || !IsOverride(method))
        {
            return;
        }
      
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
        foreach(var pluginEvent in Events)
        {
            CheckEvent(handler, handlerType, pluginEvent.methodDelegate, pluginEvent.Event);
        }
    }

    internal static void InitializeEvents()
    {
        AddEvent(nameof(XazeEventHandler.OnPlayerHearingFakePlayer), typeof(XazeEvents), nameof(XazeEvents.HearingFake));
        AddEvent(nameof(XazeEventHandler.OnPlayerScaleChanging), typeof(XazeEvents), nameof(XazeEvents.ScaleChanging));
        AddEvent(nameof(XazeEventHandler.OnPlayerHurting), typeof(XazeEvents), nameof(XazeEvents.Hurting));
        
        RegisterPluginEvents.InvokeEvent();
        Initialized = true;
    }

    internal static void InternalInvoke()
    {
        RegisterEventHandlers.InvokeEvent();
    }
}