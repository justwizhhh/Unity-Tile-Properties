using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    public class TPVariableWrapper : MonoBehaviour
    {
        [SerializeReference]
        public TPVariableType Variable;
    }
}
