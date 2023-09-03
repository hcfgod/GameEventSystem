using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalEventDistribution : IEventDistributionStrategy
{
	private MonoBehaviour monoBehaviour;
	
	public LocalEventDistribution(MonoBehaviour monoBehaviour)
	{
		this.monoBehaviour = monoBehaviour;
	}
	
	public void DistributeEvent(GameEvent gameEvent, object eventData, SharedEventState sharedState)
	{
		if(gameEvent.status.CanTrigger(gameEvent))
		{
			string category = gameEvent.eventCategory;
			string eventName = gameEvent.eventName;

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
		}
	}
}