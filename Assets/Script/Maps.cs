using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Maps : ScriptableObject
{
    [TextArea(maxLines: 100, minLines: 10)] public string Level1;

    public int MaxMovement;

}

