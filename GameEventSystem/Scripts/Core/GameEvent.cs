using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
	public string eventName;
	public string eventCategory;
	public string version = "1.0";
	public IEventStatus status;
	
	public bool HasBeenTriggered;
	public float CooldownTime;
	public float LastTriggerTime = 0;
	
	public List<ChainedEvent> ChainedEvents;
	
	public List<EventCondition> Conditions = new List<EventCondition>();
	
	void OnEnable()
	{
		LastTriggerTime = 0;
	}
}
