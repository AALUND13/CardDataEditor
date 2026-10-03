using CardDataEditor.Utils.Debug;
using ClassesManagerReborn;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardDataEditor.Patches {
    [HarmonyPatch]
    public class ClassesManagerPatch {
        static MethodBase TargetMethod() {
            MethodInfo method = AccessTools.Method(
                typeof(ClassesManager),
                "InstantiateModClasses"
            );

            return AccessTools.EnumeratorMoveNext(method);
        }

        static void Postfix(bool __result) {
            if (!__result) {
                LoggerUtils.Log(BepInEx.Logging.LogLevel.Info, "ClassesManager have initialize classes. allowing CardDataEditor config loading to continue.");
                CardDataEditor.ClassesInitialized = true;
            }
        }
    }
}
