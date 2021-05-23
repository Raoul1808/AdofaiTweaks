﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Tweak's patch system specifically designed to work at all versions of the game to avoid mod from crashing.
    /// </summary>
    internal class TweakPatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TweakPatch"/> class.
        /// </summary>
        /// <param name="PatchType">Type of the patching method.</param>
        /// <param name="attr">Attribute of the tweak's patch.</param>
        /// <param name="harmony">Harmony class to apply patch.</param>
        internal TweakPatch(Type PatchType, TweakPatchAttribute attr, Harmony harmony)
        {
#if DEBUG
            AdofaiTweaks.Logger.Log($"Created an instance of the patch with id of {attr.PatchId}");
#endif

            this.PatchType = PatchType;
            Metadata = attr;
            Harmony = harmony;

            // Did you know that ADOBase existed from way before, even before the very first version released to steam? This code is FULLY compatible!
            ClassType = typeof(ADOBase).Assembly.GetType(Metadata.ClassName);
            PatchTargetMethods = ClassType?.GetMethods().Where(m => m.Name.Equals(Metadata.MethodName));
#if DEBUG
            AdofaiTweaks.Logger.Log($"ClassType is {(ClassType != null ? "not " : "")}null");
#endif
        }

        private Harmony Harmony { get; set; }
        private Type ClassType { get; set; }
        private Type PatchType { get; set; }
        private IEnumerable<MethodInfo> PatchTargetMethods { get; set; }
        private List<MethodInfo> PatchedMethods { get; set; }

        /// <summary>
        /// Tweak's patch metadata (attribute data).
        /// </summary>
        internal TweakPatchAttribute Metadata { get; set; }

        /// <summary>
        /// Whether the patch is patched (enabled).
        /// </summary>
        internal bool IsEnabled {
            get
            {
                return PatchedMethods != null;
            }
        }

        /// <summary>
        /// Checks whether the patch is valid for current game's version.
        /// </summary>
        /// <returns>Patch's current availability in <see cref="bool"/>.</returns>
        internal bool IsValidPatch()
        {
            if ((Metadata.MinVersion <= GCNS.releaseNumber || Metadata.MinVersion == -1) &&
                (Metadata.MaxVersion >= GCNS.releaseNumber || Metadata.MaxVersion == -1) &&
                ClassType != null &&
                PatchType != null &&
                (PatchTargetMethods?.Count() ?? 0) != 0)
            {
#if DEBUG
                AdofaiTweaks.Logger.Log($"Patch {Metadata.PatchId} is valid");
#endif
                return true;
            }

#if DEBUG
            AdofaiTweaks.Logger.Log($"Patch {Metadata.PatchId} is invalid\n\nSpecific criteria check:\n" +
                $"Metadata.MinVersion <= GCNS.releaseNumber ({Metadata.MinVersion} <= {GCNS.releaseNumber}) is {Metadata.MinVersion <= GCNS.releaseNumber}\n" +
                $"Metadata.MinVersion <= GCNS.releaseNumber ({Metadata.MaxVersion} >= {GCNS.releaseNumber}) is {Metadata.MaxVersion >= GCNS.releaseNumber}\n" +
                $"ClassType is {ClassType}\n" +
                $"PatchType is {PatchType}\n" +
                $"PatchTargetMethods count is {PatchTargetMethods?.Count() ?? 0}");
#endif
            return false;
        }

        /// <summary>
        /// Patches this patch.
        /// </summary>
        internal void Patch()
        {
#if DEBUG
            AdofaiTweaks.Logger.Log($"Patching: with id of {Metadata.PatchId}");
#endif
            if (!IsEnabled)
            {
                // this patch below does not work because there is no HarmonyPatch Attribute.
                // PatchedMethods = Harmony.CreateClassProcessor(PatchType).Patch();
                foreach (MethodInfo method in PatchTargetMethods)
                {
#if DEBUG
                    AdofaiTweaks.Logger.Log($"<color=#abcdef>{Metadata.PatchId} | Patch apply trial to {ClassType.Name}.{method.Name}</color>");
#endif
                    MethodInfo prefixMethodInfo = PatchType.GetMethod("Prefix", AccessTools.all),
                        postfixMethodInfo = PatchType.GetMethod("Postfix", AccessTools.all);

                    HarmonyMethod prefixMethod = null,
                        postfixMethod = null;

                    if (prefixMethodInfo != null)
                    {
                        prefixMethod = new HarmonyMethod(prefixMethodInfo);
#if DEBUG
                        AdofaiTweaks.Logger.Log($"<color=#abcdef>{Metadata.PatchId} | Found prefix</color>");
#endif
                    }

                    if (postfixMethodInfo != null)
                    {
                        postfixMethod = new HarmonyMethod(postfixMethodInfo);
#if DEBUG
                        AdofaiTweaks.Logger.Log($"<color=#abcdef>{Metadata.PatchId} | Found postfix</color>");
#endif
                    }

                    Harmony.Patch(
                        method,
                        prefixMethod,
                        postfixMethod);
                }
#if DEBUG
                AdofaiTweaks.Logger.Log($"Patched successfully with id: {Metadata.PatchId}");
#endif
            }
        }

        /// <summary>
        /// Unpatches this patch.
        /// </summary>
        internal void Unpatch()
        {
#if DEBUG
            AdofaiTweaks.Logger.Log($"Unpatching: with id of {Metadata.PatchId}");
#endif
            if (IsEnabled)
            {
                // its theorically not possible to have a multiple patches here but I don't know what harmony does so I put a loop here instead of .First()
                // TODO: stop using loop here if possible
                PatchedMethods.ForEach((MethodInfo patchedMethod) =>
                {
#if DEBUG
                    AdofaiTweaks.Logger.Log($"Patch - {Metadata.PatchId}: patchedMethod [{patchedMethod.Name}]");
#endif
                    foreach (MethodInfo patchMethod in PatchType.GetMethods())
                    {
#if DEBUG
                        AdofaiTweaks.Logger.Log($"Patch - {Metadata.PatchId}: patchMethod [{patchMethod.Name}]");
#endif
                        Harmony.Unpatch(patchMethod, patchedMethod);
                    }
                });

                // this marks the patch as disabled
                PatchedMethods = null;
#if DEBUG
                AdofaiTweaks.Logger.Log($"Unpatched successfully with id: {Metadata.PatchId}");
#endif
            }
        }
    }
}
