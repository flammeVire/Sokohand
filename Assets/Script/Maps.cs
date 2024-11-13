using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Maps : ScriptableObject
{
    [TextArea(maxLines: 10, minLines: 10)] public string Level1;

    public int MaxMovement;

    public int GoodJobMovement;

}

