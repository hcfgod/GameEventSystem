using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingMiddleware : IEventMiddleware
{
	public bool Process(GameEvent gameEvent, ref object eventData)
	{
		Debug.Log($"Event triggered: {gameEvent.eventName} with data: {eventData}");
		return true;
	}
}
