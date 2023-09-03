using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeEventStatus : IEventStatus
{
	public bool CanTrigger(GameEvent gameEvent)
	{
		// Custom logic to determine if a one-time event can be triggered
		return !gameEvent.HasBeenTriggered;
	}
}
