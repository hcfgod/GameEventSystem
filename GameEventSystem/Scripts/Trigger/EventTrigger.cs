﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventTrigger
{
	private EventSubscriber eventSubscriber;
	private SharedEventState sharedState;
	private MonoBehaviour monoBehaviour;
	
	public IEventDistributionStrategy EventDistributionStrategy { get; set; }

	public List<IEventMiddleware> Middlewares { get; set; } = new List<IEventMiddleware>();

	public EventTrigger(SharedEventState sharedState, MonoBehaviour monoBehaviour)
	{
		this.sharedState = sharedState;
		this.monoBehaviour = monoBehaviour;
	}
	
	public void TriggerEvent(GameEvent gameEvent, object eventData, bool useThreadSafeOperations = false, EventQueueManager eventQueueManager = null)
	{
		foreach (var middleware in Middlewares)
		{
			if (!middleware.Process(gameEvent, ref eventData))
			{
				return;
			}
		}
    
		if (sharedState == null)
		{
			throw new GameEventException("SharedState is null.");
		}

		if (useThreadSafeOperations && eventQueueManager == null)
		{
			throw new GameEventException("EventQueueManager is null but thread-safe operations are requested.");
		}
    
		Action actualTriggerEventAction = () =>
		{
			try
			{
				float currentTime = Time.time;
        
				if (currentTime - gameEvent.LastTriggerTime < gameEvent.CooldownTime)
				{
					return;
				}
        
				foreach (var condition in gameEvent.Conditions)
				{
					if (condition != null && !condition(gameEvent, eventData))
					{
						return;
					}
				}

				if(gameEvent.status.CanTrigger(gameEvent))
				{
					EventDistributionStrategy?.DistributeEvent(gameEvent, eventData, sharedState);
            
					foreach (var chainedEvent in gameEvent.ChainedEvents)
					{
						monoBehaviour.StartCoroutine(TriggerChainedEvent(chainedEvent, eventData));
					}
					
					if(gameEvent.CooldownTime > 0)
						gameEvent.LastTriggerTime = currentTime;
				}
			}
				catch(GameEventException gameEventException)
				{
					Debug.LogError($"An error occurred: {gameEventException.Message}");
				}
				catch(Exception e)
				{
					Debug.LogError($"An error occurred while triggering the event: {e.Message}");
				}
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

	public void TriggerEventWithDelay(GameEvent gameEvent, object eventData, float delay, string customID, bool useThreadSafeOperations = false, EventQueueManager eventQueueManager = null)
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
			Coroutine coroutine = monoBehaviour.StartCoroutine(DelayedEvent(gameEvent, eventData, delay, customID, coroutineID));
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
	
	private IEnumerator DelayedEvent(GameEvent gameEvent, object eventData, float delay, string customID, string coroutineID) 
	{
		string category = gameEvent.eventCategory;
		string eventName = gameEvent.eventName;
		
		yield return new WaitForSeconds(delay);

		if (sharedState.RunningDelayedEvents.ContainsKey(eventName) && 
			sharedState.RunningDelayedEvents[eventName].ContainsKey(customID)) 
		{
			sharedState.RunningDelayedEvents[eventName].Remove(customID);
		}

		TriggerEvent(gameEvent, eventData);
	}
	
	// This part remains the same as your previous implementation
	private IEnumerator TriggerChainedEvent(ChainedEvent chainedEvent, object eventData)
	{
		yield return new WaitForSeconds(chainedEvent.Delay);
		TriggerEvent(chainedEvent.Event, eventData);
	}
}
