using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ControlScheme", menuName = "Control Scheme", order = 51)]
public class ControlScheme : ScriptableObject
{
    public KeyCode Up, Down, Left, Right;
    public KeyCode Kick;
}
