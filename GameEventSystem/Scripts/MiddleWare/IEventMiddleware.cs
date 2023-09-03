using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventMiddleware
{
	bool Process(GameEvent gameEvent, ref object eventData);
}
