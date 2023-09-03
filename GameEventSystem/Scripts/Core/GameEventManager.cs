using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventManager : MonoBehaviour
{
	[SerializeField] private bool useThreadSafeOperations = false;
	[SerializeField] private bool isGlobalManager = true;

	private Queue<Action> mainThreadQueue = new Queue<Action>();
	
	private SharedEventState sharedState = new SharedEventState();
	private EventQueueManager eventQueueManager = new EventQueueManager();
	private EventSubscriber eventSubscriber;
	private EventTrigger eventTrigger;
	
	private static GameEventManager instance;
	public static GameEventManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameEventManager>();
				if (instance == null)
				{
					GameObject go = new GameObject("GameEventManager");
					instance = go.AddComponent<GameEventManager>();
				}
			}
			return instance;
		}
	}

	
	private void Awake()
	{
		if (isGlobalManager)
		{
			if (Instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			instance = this;  // This is a local manager; we don't call DontDestroyOnLoad.
		}

		eventSubscriber = new EventSubscriber(sharedState, eventQueueManager);
		eventTrigger = new EventTrigger(sharedState, this);
		eventTrigger.Middlewares.Add(new LoggingMiddleware());
		eventTrigger.Middlewares.Add(new ValidationMiddleware());
	}
	
	void Update() 
	{
		if (useThreadSafeOperations)
		{
			eventQueueManager.ExecuteAll();
		}
	}
	
	public void EnableCategory(string category)
	{
		sharedState.DisabledCategories.Remove(category);
	}
	
	public void DisableCategory(string category)
	{
		if (sharedState.Events.ContainsKey(category))
		{
			sharedState.DisabledCategories.Add(category);
		}
	}


	public void Subscribe(GameEvent gameEvent, Action<object> listener, int priority = 0)
	{		
		eventSubscriber.Subscribe(gameEvent, listener, priority, useThreadSafeOperations);
	}

	public void Unsubscribe(GameEvent gameEvent, Action<object> listener, int priority = 0)
	{	
		eventSubscriber.Unsubscribe(gameEvent, listener, priority, useThreadSafeOperations);
	}

	public void TriggerEvent(GameEvent gameEvent, object eventData)
	{	
		eventTrigger.TriggerEvent(gameEvent, eventData, useThreadSafeOperations, eventQueueManager);
	}
	
	public void TriggerEventWithDelay(GameEvent gameEvent, object eventData, float delay, string customID)
	{
		eventTrigger.TriggerEventWithDelay(gameEvent, eventData, delay, customID, useThreadSafeOperations, eventQueueManager);
	}
	
	public void CancelDelayedEvent(GameEvent gameEvent, string customID = null)
	{
		string eventName = gameEvent.eventName;

		if (sharedState.RunningDelayedEvents.ContainsKey(eventName))
		{
			if (customID == null)
			{
				foreach (var idCoroutines in sharedState.RunningDelayedEvents[eventName])
				{
					foreach (var coroutine in idCoroutines.Value.Values)
					{
						StopCoroutine(coroutine);
					}
				}
				sharedState.RunningDelayedEvents[eventName].Clear();
				Debug.Log($"All delayed events with the name {eventName} were canceled.");
			}
			else
			{
				if (sharedState.RunningDelayedEvents[eventName].ContainsKey(customID))
				{
					foreach (var coroutine in sharedState.RunningDelayedEvents[eventName][customID].Values)
					{
						StopCoroutine(coroutine);
					}
					sharedState.RunningDelayedEvents[eventName].Remove(customID);
					Debug.Log($"Delayed events with the name {eventName} and custom ID {customID} were canceled.");
				}
				else
				{
					Debug.LogWarning($"No running delayed event with the name {eventName} and custom ID {customID} found.");
				}
			}
		}
		else
		{
			Debug.LogWarning($"No running delayed events with the name {eventName} found.");
		}
	}
	
	public GameEvent CreateEvent(string eventName, string eventCategory, IEventStatus status)
	{
		GameEvent newEvent = ScriptableObject.CreateInstance<GameEvent>();
		newEvent.eventName = eventName;
		newEvent.eventCategory = eventCategory;
		newEvent.status = status;
    
		// You could return the new GameEvent for further use if needed
		return newEvent;
	}
	
	public void Cleanup()
	{
		eventSubscriber.Cleanup();
		eventQueueManager.ClearQueue();
	}
}
