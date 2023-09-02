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
				sharedState.RunningDelayedEvents[eventName] = new Dictionary<string, List<Coroutine>>();
			}

			if(!sharedState.RunningDelayedEvents[eventName].ContainsKey(customID))
			{
				sharedState.RunningDelayedEvents[eventName][customID] = new List<Coroutine>();
			}

			Coroutine coroutine = monoBehaviour.StartCoroutine(DelayedEvent(gameEvent, data, delay, customID));
			sharedState.RunningDelayedEvents[eventName][customID].Add(coroutine);
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
	
	private IEnumerator DelayedEvent(GameEvent gameEvent, object data, float delay, string customID)
	{
		string category = gameEvent.eventCategory;
		string eventName = gameEvent.eventName;
		
		yield return new WaitForSeconds(delay);

		if (sharedState.RunningDelayedEvents.ContainsKey(gameEvent.eventName))
		{
			sharedState.RunningDelayedEvents[gameEvent.eventName].Remove(customID);
		}

		TriggerEvent(gameEvent, data);
	}
}
