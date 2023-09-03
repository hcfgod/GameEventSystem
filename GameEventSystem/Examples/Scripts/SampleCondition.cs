using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCondition : MonoBehaviour
{
	public GameEvent healthChangedEvent;

	private void Start()
	{
		healthChangedEvent.Conditions.Add(HealthBelowFifty);
	}

	private bool HealthBelowFifty(GameEvent gameEvent, object eventData)
	{
		float health = (float)eventData;
		return health <= 50;
	}
}
