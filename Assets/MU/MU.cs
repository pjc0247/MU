using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MU
{
    private class SubscriberInfo
    {
        public MethodInfo method;
        public MonoBehaviour obj;
    }

    private static Dictionary<string, List<SubscriberInfo>> subscribers = new Dictionary<string, List<SubscriberInfo>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        var asm = Assembly.GetEntryAssembly();
        var behaviours = asm.GetTypes().Where(
            x => x.IsSubclassOf(typeof(MonoBehaviour)));

        foreach (var type in behaviours)
        {
            foreach (var method in type.GetMethods())
            {
                var subscribeAttr = method.GetCustomAttributes(true)
                    .Where(x => x.GetType().IsSubclassOf(typeof(SubscribeAttribute)))
                    .FirstOrDefault();

                if (subscribeAttr != null)
                {

                }
            }
        }
    }

    public static void AddSubscriber(string name, MonoBehaviour obj, MethodInfo method)
    {
        if (subscribers.ContainsKey(name) == false)
            subscribers[name] = new List<SubscriberInfo>();

        subscribers[name].Add(new SubscriberInfo()
        {
            method = method,
            obj = obj
        });
    }
    public static void RemoveSubscriber(string name, MonoBehaviour obj, MethodInfo method)
    {
        if (subscribers.ContainsKey(name) == false)
            return;

        var subscriber = subscribers[name]
            .Find(x => x.method == method && x.obj == obj);
        if (subscriber == null)
            return;

        subscribers[name].Remove(subscriber);
    }

    public static void AddBehaviour(MonoBehaviour obj)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var method in obj.GetType().GetMethods(flags))
        {
            var subscribeAttr = (SubscribeAttribute)method.GetCustomAttributes(true)
                    .Where(x => x.GetType().IsSubclassOf(typeof(SubscribeAttribute)))
                    .FirstOrDefault();

            if (subscribeAttr != null)
                AddSubscriber(subscribeAttr.name, obj, method);
        }
    }
    public static void RemoveBehaviour(MonoBehaviour obj)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var method in obj.GetType().GetMethods(flags))
        {
            var subscribeAttr = (SubscribeAttribute)method.GetCustomAttributes(true)
                    .Where(x => x.GetType().IsSubclassOf(typeof(SubscribeAttribute)))
                    .FirstOrDefault();

            if (subscribeAttr != null)
                RemoveSubscriber(subscribeAttr.name, obj, method);
        }
    }

    public static void Publish(string name, object arg)
    {
        Publish(name, arg);
    }
    public static void Publish(string name, params object[] args)
    {
        if (subscribers.ContainsKey(name) == false)
            return;

        foreach (var subscriber in subscribers[name])
            subscriber.method.Invoke(subscriber.obj, args);
    }
}
