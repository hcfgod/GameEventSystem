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
		if (gameEvent == null)
		{
			Debug.LogError("GameEvent is null.");
			return;
		}

		if (listener == null)
		{
			Debug.LogError("Listener is null.");
			return;
		}

		if (sharedState == null)
		{
			Debug.LogError("SharedState is null.");
			return;
		}

		if (useThreadSafeOperations && eventQueueManager == null)
		{
			Debug.LogError("EventQueueManager is null but thread-safe operations are requested.");
			return;
		}
		
		Action actualSubscribeAction = () =>
		{
			try
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
			}
		    catch(Exception e)
			{
				Debug.LogError($"An error occurred while subscribing to the event: {e.Message}");
			}
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
	
	// New method to clean up unused entries
	public void Cleanup() 
	{
		foreach (var category in sharedState.Events.Keys) 
		{
			foreach (string eventName in sharedState.Events[category].Keys) 
			{
				if (sharedState.Events[category][eventName].Count == 0) 
				{
					sharedState.Events[category].Remove(eventName);
				}
			}
			if (sharedState.Events[category].Count == 0) {
				sharedState.Events.Remove(category);
			}
		}
	}
}
