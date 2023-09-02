using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
	public string eventName;
	public string eventCategory;
	public IEventStatus status;
	
	public bool HasBeenTriggered { get; set; }
	public float CooldownTime;
	public float LastTriggerTime { get; set; }
	
	public List<ChainedEvent> ChainedEvents;
}
