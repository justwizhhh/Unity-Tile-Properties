using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace TileProperties
{
    public static class TPDrawerRegistry
    {
        private static readonly Dictionary<System.Type, ITPDrawer> variable_drawers =
            new Dictionary<System.Type, ITPDrawer>
        {
        { typeof(TPIntVariable), new TPIntDrawer() },
        { typeof(TPFloatVariable), new TPFloatDrawer() },
        { typeof(TPBoolVariable), new TPBoolDrawer() },
        { typeof(TPCharVariable), new TPCharDrawer() },
        { typeof(TPStringVariable), new TPStringDrawer() },
        { typeof(TPVector2Variable), new TPVector2Drawer() },
        { typeof(TPVector2IntVariable), new TPVector2IntDrawer() },
        { typeof(TPVector3Variable), new TPVector3Drawer() },
        { typeof(TPVector3IntVariable), new TPVector3IntDrawer() },
        { typeof(TPColorVariable), new TPColorDrawer() },
        { typeof(TPAudioVariable), new TPAudioDrawer() },
        { typeof(TPTextureVariable), new TPTextureDrawer() },
        { typeof(TPParticlesVariable), new TPParticleDrawer() },
        { typeof(TPGameObjectVariable), new TPGameObjectDrawer() }
        };

        public static ITPDrawer GetDrawer(SerializedProperty variable)
        {
            Assembly asm = typeof(TPVariableType).Assembly;
            System.Type type = asm.GetType(variable.managedReferenceFullTypename.Substring(15));
            if (type == null)
            {
                return null;
            }
            return variable_drawers[type];
        }
    }
}
