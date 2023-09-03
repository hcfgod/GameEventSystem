using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventDistributionStrategy
{
	void DistributeEvent(GameEvent gameEvent, object eventData, SharedEventState sharedState);
}
