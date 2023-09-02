using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventSubscriber
{
	private EventQueueManager eventQueueManager;
	private SharedEventState sharedState;
	
	public EventSubscriber(SharedEventState sharedState, EventQueueManager eventQueueManager)
	{
		this.sharedState = sharedState;
		this.eventQueueManager = eventQueueManager;
	}

	public void Subscribe(GameEvent gameEvent, Action<object> listener, int priority = 0, bool useThreadSafeOperations = false)
	{
		Action actualSubscribeAction = () =>
		{
			string category = gameEvent.eventCategory;
			string eventName = gameEvent.eventName;

			if (!sharedState.Events.ContainsKey(category))
			{
				sharedState.Events[category] = new Dictionary<string, SortedDictionary<int, Action<object>>>();
			}

			if (!sharedState.Events[category].ContainsKey(eventName))
			{
				sharedState.Events[category][eventName] = new SortedDictionary<int, Action<object>>();
			}

			if (!sharedState.Events[category][eventName].ContainsKey(priority))
			{
				sharedState.Events[category][eventName][priority] = null;
			}

			sharedState.Events[category][eventName][priority] += listener;
		};

		if (useThreadSafeOperations)
		{
			eventQueueManager.EnqueueAction(actualSubscribeAction);
		}
		else
		{
			actualSubscribeAction();
		}
	}

	public void Unsubscribe(GameEvent gameEvent, Action<object> listener, int priority = 0, bool useThreadSafeOperations = false)
	{
		Action actualUnsubscribeAction = () =>
		{
			string category = gameEvent.eventCategory;
			string eventName = gameEvent.eventName;
		
			if (sharedState.Events.ContainsKey(category) && sharedState.Events[category].ContainsKey(gameEvent.eventName) && sharedState.Events[category][gameEvent.eventName].ContainsKey(priority))
			{
				sharedState.Events[category][gameEvent.eventName][priority] -= listener;
			}
		};

		if (useThreadSafeOperations)
		{
			eventQueueManager.EnqueueAction(actualUnsubscribeAction);
		}
		else
		{
			actualUnsubscribeAction();
		}
	}
}
