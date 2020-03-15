using System.Collections;
using System.Collections.Generic;

public class EventSubscriber
{
    string tag;
    EventManager.GameFunction function;
    public EventSubscriber(string givenTag, EventManager.GameFunction givenFunction)
    {
        tag = givenTag;
        function = givenFunction;
        EventManager.SubscribeToEvent(tag, function);
    }

    public void Destroy()
    {
        EventManager.UnsubscribeToEvent(tag, function);
        function = null;
    }
}

public class EventManager
{
    public delegate void GameFunction(object obj);

    //the event manager has a dictionary which binds an event type enum with a delegate
    private static Dictionary<string, GameFunction> subscriptionList = new Dictionary<string, GameFunction>();

    /// <summary>
    /// Calls all functions which have subscribed to givenEventType
    /// </summary>
    /// <param name="givenTag">string symbolizing which event is being called</param>
    /// <param name="obj">Defaults to null. The parameter which will be given to the called functions.</param>
    public static void BroadcastEvent(string givenTag, object obj = null)
    {
        if (subscriptionList.TryGetValue(givenTag, out GameFunction subscribers))
        {
            subscribers?.Invoke(obj);
        }
    }

    ///<summary>
    /// Adds givenFunction to the delegate called on givenEventType 
    /// - you MUST use UnsubscribeToEvent on destroy of your object, otherwise you WILL face memory leaks
    ///</summary>
    ///<param name="givenTag"> Tag on which the function will be called.</param> 
    ///<param name="givenFunction"> Function taking an object as parameter which will be called on BroadcastEvent.</param> 
    public static void SubscribeToEvent(string givenTag, GameFunction givenFunction)
    {
        if (!subscriptionList.ContainsKey(givenTag))
        {
            subscriptionList[givenTag] = givenFunction;
        }
        else
        {
            subscriptionList[givenTag] += givenFunction;
        }
    }

    ///<summary>
    ///if an object doesn't unsubscribe from an event on destroy, the event manager will still hold a pointer to said object, meaning said object will not be garbage collected
    ///</summary>
    ///<param name="givenEventType"> Tag on which the function was be called.</param> 
    ///<param name="givenFunction"> Function which the calling object was subscribing with.</param> 
    public static void UnsubscribeToEvent(string givenEventType, GameFunction givenFunction)
    {
        if (subscriptionList.ContainsKey(givenEventType))
        {
            subscriptionList[givenEventType] -= givenFunction;
        }
    }
}