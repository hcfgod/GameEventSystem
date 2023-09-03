using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SharedEventState
{
	public HashSet<string> DisabledCategories = new HashSet<string>();
	public Dictionary<string, Dictionary<string, SortedDictionary<int, Action<object>>>> Events = new Dictionary<string, Dictionary<string, SortedDictionary<int, Action<object>>>>();
	public Dictionary<string, Dictionary<string, Dictionary<string, Coroutine>>> RunningDelayedEvents = new Dictionary<string, Dictionary<string, Dictionary<string, Coroutine>>>();
}
