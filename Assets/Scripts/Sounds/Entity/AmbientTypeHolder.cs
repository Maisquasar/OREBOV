using System;
using UnityEngine;

public enum AmbientSoundType
{
    Exterior,
    Interior,
    Rain,
}

public class AmbientTypeHolder : MonoBehaviour
{
    [SerializeField] public AmbientSoundType AmbientType;
}
