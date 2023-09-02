using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
	public string eventName;
	public string eventCategory;
	
	public EventStatus status; // Add this line at the class level
}
