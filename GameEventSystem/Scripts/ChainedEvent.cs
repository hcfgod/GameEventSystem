using UnityEngine;

[CreateAssetMenu(fileName = "New Chained Event", menuName = "Chained Event")]
public class ChainedEvent : ScriptableObject
{
	public GameEvent Event;
	public float Delay;
}