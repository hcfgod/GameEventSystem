using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameEvent OnHealthChangedEvent;
	public float health = 100f;

	protected void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			health -= 10;
			GameEventManager.Instance.TriggerEvent(OnHealthChangedEvent, health);
		}
	}
}
