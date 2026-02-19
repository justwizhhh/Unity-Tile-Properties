using System.Collections.Generic;
using UnityEngine;

namespace TileProperties
{
    internal class TPVariableTypeRegistry
    {
        // --------------------------------------------
        //
        // This class stores a list of every variable type that can be made into a Tile Property, as well
        // as its non-Tile-Properties type variant, for easier adding and removing without specifying the
        // specific TP versions of each type
        //
        // --------------------------------------------

        private static readonly Dictionary<System.Type, System.Type> variable_variants =
            new Dictionary<System.Type, System.Type>
        {
        { typeof(int), typeof(TPIntVariable) },
        { typeof(float), typeof(TPFloatVariable) },
        { typeof(bool), typeof(TPBoolVariable) },
        { typeof(char), typeof(TPCharVariable) },
        { typeof(string), typeof(TPStringVariable) },
        { typeof(Vector2), typeof(TPVector2Variable) },
        { typeof(Vector2Int), typeof(TPVector2IntVariable) },
        { typeof(Vector3), typeof(TPVector3Variable) },
        { typeof(Vector3Int), typeof(TPVector3IntVariable) },
        { typeof(Vector4), typeof(TPVector4Variable) },
        { typeof(Quaternion), typeof(TPQuaternionVariable) },
        { typeof(Rect), typeof(TPRectVariable) },
        { typeof(RectInt), typeof(TPRectIntVariable) },
        { typeof(Bounds), typeof(TPBoundsVariable) },
        { typeof(BoundsInt), typeof(TPBoundsIntVariable) },
        { typeof(Color), typeof(TPColorVariable) },
        { typeof(AudioClip), typeof(TPAudioVariable) },
        { typeof(Texture), typeof(TPTextureVariable) },
        { typeof(Material), typeof(TPMaterialVariable) },
        { typeof(PhysicsMaterial), typeof(TPPhysicsMaterialVariable) },
        { typeof(ParticleSystem), typeof(TPParticlesVariable) },
        { typeof(GameObject), typeof(TPGameObjectVariable) }
        };

        public static System.Type GetTPVariableType(System.Type TP_type)
        {
            return variable_variants[TP_type];
        }
    }
}
