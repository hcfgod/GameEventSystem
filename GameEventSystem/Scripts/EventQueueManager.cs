using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventQueueManager
{
	private Queue<Action> mainThreadQueue = new Queue<Action>();

	public void EnqueueAction(Action action)
	{
		mainThreadQueue.Enqueue(action);
	}

	public void ExecuteAll()
	{
		while (mainThreadQueue.Count > 0)
		{
			mainThreadQueue.Dequeue().Invoke();
		}
	}
}
