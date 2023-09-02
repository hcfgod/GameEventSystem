using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCondition : MonoBehaviour
{
	public GameEvent healthChangedEvent;

	private void Start()
	{
		healthChangedEvent.Conditions.Add(HealthBelowFifty);
		healthChangedEvent.Conditions.Add(HealthAboveTen);
	}

	private bool HealthBelowFifty(GameEvent gameEvent, object eventData)
	{
		float health = (float)eventData;
		return health <= 50;
	}

	private bool HealthAboveTen(GameEvent gameEvent, object eventData)
	{
		float health = (float)eventData;
		return health > 10;
	}
}
