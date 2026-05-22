using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class UnitTemplateWrapper
    {
        public UnitBlueprint Blueprint { get; set; }

        public Echelon Echelon { get; set; }

        public Echelon PromotionPool { get; set; }

        public List<PositionBlueprint> PositionBlueprints { get; set; }

        public List<UnitTemplateWrapper> SubUnits { get; set; }

        private static Dictionary<int, UnitTemplateWrapper> Cache { get; set; }

        public UnitTemplateWrapper(UnitBlueprint blueprint)
        {
            Blueprint = blueprint;
            PositionBlueprints = blueprint.PositionBlueprints.ToList();
            Echelon = blueprint.Echelon;
            PromotionPool = blueprint.PromotionEchelon;

            SubUnits = new List<UnitTemplateWrapper>();
            foreach (UnitBlueprintAttachment attachement in blueprint.SubUnitBlueprints)
            {
                // Skip attachments where this unit is the child
                if (attachement.ParentId == blueprint.Id)
                {
                    for (int i = 0; i < attachement.Count; i++)
                        SubUnits.Add(FetchCache(attachement.Child));
                }
            }
        }

        static UnitTemplateWrapper()
        {
            Cache = new Dictionary<int, UnitTemplateWrapper>();
        }

        public static UnitTemplateWrapper FetchCache(UnitBlueprint template)
        {
            if (!Cache.ContainsKey(template.Id))
                Cache.Add(template.Id, new UnitTemplateWrapper(template));

            return Cache[template.Id];
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
