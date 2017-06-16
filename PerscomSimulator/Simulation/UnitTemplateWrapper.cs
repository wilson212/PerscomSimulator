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
        public UnitTemplate Template { get; set; }

        public Echelon Echelon { get; set; }

        public Echelon PromotionPool { get; set; }

        public List<Billet> Billets { get; set; }

        public List<UnitTemplateWrapper> SubUnits { get; set; }

        private static Dictionary<int, UnitTemplateWrapper> Cache { get; set; }

        public UnitTemplateWrapper(UnitTemplate template)
        {
            Template = template;
            Billets = template.Billets.ToList();
            Echelon = template.Echelon;
            PromotionPool = template.PromotionPool;

            SubUnits = new List<UnitTemplateWrapper>();
            foreach (UnitTemplateAttachment attachement in template.UnitTemplateAttachments)
            {
                // Skip attachments where this unit is the child
                if (attachement.ParentId == template.Id)
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

        public static UnitTemplateWrapper FetchCache(UnitTemplate template)
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
