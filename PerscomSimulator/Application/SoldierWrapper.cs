using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perscom.Properties;

namespace Perscom
{
    public class SoldierWrapper
    {
        public Soldier Soldier;

        protected DateTime ReportDate { get; set; }

        [DisplayName("SID")]
        public int SpawnId => Soldier.SpawnId;

        public Image RankIcon
        {
            get
            {
                char type = Soldier.RankInfo.TypeCode;
                string code = $"{type}{Soldier.RankInfo.Grade}.png";
                return ImageAccessor.GetImage(Path.Combine("icons", code));
            }
        }

        public Image RankImage
        {
            get
            {
                char type = Soldier.RankInfo.TypeCode;
                string code = $"{type}{Soldier.RankInfo.Grade}.png";
                return ImageAccessor.GetImage(Path.Combine("large", code));
            }
        }

        public string Name =>  $"{Soldier.FirstName} {Soldier.LastName}";

        [DisplayName("Service Entry Date")]
        public DateTime ServiceEntryDate => Soldier.ServiceEntryDate;

        [DisplayName("Time In Service")]
        public int TimeInService => Soldier.ServiceEntryDate.MonthDifference(ReportDate);

        [DisplayName("Time In Grade")]
        public int TimeInGrade => Soldier.LastPromotionDate.MonthDifference(ReportDate);

        public SoldierWrapper(Soldier s, DateTime reportDate)
        {
            Soldier = s;
            ReportDate = reportDate;
        }
    }
}
