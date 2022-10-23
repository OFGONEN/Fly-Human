using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "event_stickman_collide", menuName = "FF/Event/Stickman Collide" ) ]
public class StickmanGameEvent : GameEvent
{
    public Stickman event_stickman;

    public void Raise( Stickman value )
    {
		event_stickman = value;
		Raise();
	}
}