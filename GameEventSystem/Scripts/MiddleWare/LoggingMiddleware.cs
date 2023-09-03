using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingMiddleware : IEventMiddleware
{
	public bool Process(GameEvent gameEvent, ref object eventData)
	{
		if(gameEvent.status == null)
		{
			gameEvent.status = new OneTimeEventStatus();
		}
				
		float currentTime = Time.time;
			
		if (currentTime - gameEvent.LastTriggerTime < gameEvent.CooldownTime)
		{
			return false;
		}
			
		foreach (var condition in gameEvent.Conditions)
		{
			if (condition != null && !condition(gameEvent, eventData))
			{
				return false;
			}
		}
		
		if(!gameEvent.status.CanTrigger(gameEvent))
		{
			
			return false;
		}
		
		Debug.Log($"Event triggered: {gameEvent.eventName} with data: {eventData}");
		return true;
	}
}
