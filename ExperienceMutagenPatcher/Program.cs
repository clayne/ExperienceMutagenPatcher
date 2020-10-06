using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ExperienceMutagenPatcher
{
    public class Program
    {
        private static string loadPath = "";
        public static int Main(string[] args)
        {
            var done = SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher
                    {
                        TargetRelease = GameRelease.SkyrimSE
                    }
                }); ;
            File.Delete(loadPath + @"\Null");
            return done;
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var csv = new StringBuilder();
            foreach (var race in state.LoadOrder.PriorityOrder.WinningOverrides<IRaceGetter>())
            {
                float startingHealth = race.Starting.Values.ElementAt(0);
                csv.Append(race.EditorID + "," + Math.Round(startingHealth / 10) + '\n');
            }
            loadPath = state.Settings.DataFolderPath;
            Directory.CreateDirectory(state.Settings.DataFolderPath + @"\SKSE\Plugins\Experience\Races\");
            File.WriteAllText(state.Settings.DataFolderPath + @"\SKSE\Plugins\Experience\Races\experiencePatch.csv", csv.ToString());
        }
    }
}
