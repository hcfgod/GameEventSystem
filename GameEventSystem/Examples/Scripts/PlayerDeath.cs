using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
	public GameEvent OnHealthChangedEvent;
	public GameEvent PlayDeathEvent;

	private void OnEnable()
	{
		GameEventManager.Instance.Subscribe(OnHealthChangedEvent, PlayerHealthChanged);
		GameEventManager.Instance.Subscribe(PlayDeathEvent, OnPlayerDeath);
	}

	private void OnDisable()
	{
		GameEventManager.Instance.Unsubscribe(OnHealthChangedEvent, PlayerHealthChanged);
		GameEventManager.Instance.Unsubscribe(PlayDeathEvent, OnPlayerDeath);
	}

	public void PlayerHealthChanged(object data)
	{
		float playerHealth = (float)data;
		Debug.Log("Current Player Health: " + playerHealth);
	}
	
	private void OnPlayerDeath(object data)
	{
		float playerHealth = (float)data;
		
		if (playerHealth <= 0)
		{
			Debug.Log("Player Is Dead.");
		}
	}
}
