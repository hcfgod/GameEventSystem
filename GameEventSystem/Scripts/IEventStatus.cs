using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventStatus
{
	bool CanTrigger(GameEvent gameEvent);
}
