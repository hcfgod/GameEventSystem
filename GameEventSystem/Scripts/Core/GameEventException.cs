using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventException : Exception
{
	public GameEventException(string message) : base(message)
	{
	}
}
