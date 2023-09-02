using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventTrigger
{
	private EventSubscriber eventSubscriber;
	private SharedEventState sharedState;
	private MonoBehaviour monoBehaviour;
	
	public EventTrigger(SharedEventState sharedState, MonoBehaviour monoBehaviour)
	{
		this.sharedState = sharedState;
		this.monoBehaviour = monoBehaviour;
	}

	public void TriggerEvent(GameEvent gameEvent, object eventData, bool useThreadSafeOperations = false, EventQueueManager eventQueueManager = null)
	{
		Action actualTriggerEventAction = () =>
		{
			string category = gameEvent.eventCategory;
			string eventName = gameEvent.eventName;

			if (gameEvent.status.HasFlag(EventStatus.OneTime) && gameEvent.status.HasFlag(EventStatus.Triggered))
			{
				// Skip triggering the event as it's a one-time event that has been triggered already
				return;
			}
	
			if (sharedState.DisabledCategories.Contains(category))
			{
				return;
			}
	
			if (sharedState.Events.ContainsKey(category) && sharedState.Events[category].ContainsKey(gameEvent.eventName))
			{
				foreach (var kvp in sharedState.Events[category][gameEvent.eventName])
				{
					kvp.Value?.Invoke(eventData);
				}
			}

			gameEvent.status |= EventStatus.Triggered; // Mark the event as triggered
		};

		if (useThreadSafeOperations)
		{
			eventQueueManager.EnqueueAction(actualTriggerEventAction);
		}
		else
		{
			actualTriggerEventAction();
		}
	}

	public void TriggerEventWithDelay(GameEvent gameEvent, object data, float delay, string customID, bool useThreadSafeOperations = false, EventQueueManager eventQueueManager = null)
	{
		Action actualTriggerEventWithDelayAction = () =>
		{
			string category = gameEvent.eventCategory;
			string eventName = gameEvent.eventName;
        
			if (!sharedState.RunningDelayedEvents.ContainsKey(eventName))
			{
    			sharedState.RunningDelayedEvents[eventName] = new Dictionary<string, Dictionary<string, Coroutine>>();
			}

			if (!sharedState.RunningDelayedEvents[eventName].ContainsKey(customID))
			{
    			sharedState.RunningDelayedEvents[eventName][customID] = new Dictionary<string, Coroutine>();
			}

			string coroutineID = System.Guid.NewGuid().ToString();
			Coroutine coroutine = monoBehaviour.StartCoroutine(DelayedEvent(gameEvent, data, delay, customID, coroutineID));
			sharedState.RunningDelayedEvents[eventName][customID][coroutineID] = coroutine;
			
		};
  
		if (useThreadSafeOperations)
		{
			eventQueueManager.EnqueueAction(actualTriggerEventWithDelayAction);
		}
		else
		{
			actualTriggerEventWithDelayAction();
		}
	}
	
	private IEnumerator DelayedEvent(GameEvent gameEvent, object data, float delay, string customID, string coroutineID) 
	{
		string category = gameEvent.eventCategory;
		string eventName = gameEvent.eventName;
		
		yield return new WaitForSeconds(delay);

		if (sharedState.RunningDelayedEvents.ContainsKey(eventName) && 
			sharedState.RunningDelayedEvents[eventName].ContainsKey(customID)) 
		{
			sharedState.RunningDelayedEvents[eventName].Remove(customID);
		}

		TriggerEvent(gameEvent, data);
	}
}
