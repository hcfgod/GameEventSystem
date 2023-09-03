using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationMiddleware : IEventMiddleware
{
	public bool Process(GameEvent gameEvent, ref object eventData)
	{
		if (gameEvent == null)
		{
			throw new GameEventException("GameEvent is null.");
		}

		if (eventData == null)
		{
			throw new GameEventException("Event data is null.");
		}
		
		return true;
	}
}






