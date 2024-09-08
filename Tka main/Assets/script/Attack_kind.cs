using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack_kind", menuName = "Attack_kind")]
public class Attack_kind : ScriptableObject
{
    public float damage;
    public float speed =0.4f;
    public AnimatorOverrideController animator;
}
