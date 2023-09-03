using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public interface IEventStatus
{
	bool CanTrigger(GameEvent gameEvent);
}
