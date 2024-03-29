﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;


namespace SaleOfGoods.Patches
{
    [HarmonyPatch(typeof(Toils_Recipe), "CalculateIngredients")]
    static class Toils_Recipe_CalculateIngredients_SaleOfGoodsPatch
    {
        static bool doStripCorpse(RecipeDef def, Thing thing)
        {
            if (!def.autoStripCorpses)
            {
                IStrippable strippable = thing as IStrippable;
                Corpse corpse = thing as Corpse;
                Pawn pawn;
                if (corpse == null)
                    pawn = thing as Pawn;
                else
                    pawn = corpse.InnerPawn;

                return strippable != null && thing.MapHeld != null && (thing.MapHeld.designationManager.DesignationOn(thing, DesignationDefOf.Strip) != null);
            }
            else
                return true;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
        {
            FieldInfo f = AccessTools.Field(typeof(RecipeDef), "autoStripCorpses");
            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == f)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)4);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Toils_Recipe_CalculateIngredients_SaleOfGoodsPatch), nameof(Toils_Recipe_CalculateIngredients_SaleOfGoodsPatch.doStripCorpse)));
                }
                else
                {
                    yield return i;
                }

            }
        }
    }
}
