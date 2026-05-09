namespace Perscom.AI.Dtos;

public class PositionBlueprintDto
{
    public string Name { get; set; }
    public int UnitBlueprintId { get; set; }
    public int CatagoryId { get; set; }
    public int TargetRankId { get; set; }
    public int? PositionalRankId { get; set; }
    public string Flag { get; set; } = "NormalAssignment";
    public int PromotionEchelonId { get; set; }
    public int OccupationId { get; set; }
    public int Stature { get; set; } = 0;
    public int Prestige { get; set; } = 50;
    public int MinTourLength { get; set; } = 0;
    public int MaxTourLength { get; set; } = 0;
    public bool CanRetireEarly { get; set; } = true;
    public bool CanBePromotedEarly { get; set; } = true;
    public bool CanLateralEarly { get; set; } = false;
    public bool Waiverable { get; set; } = true;
    public string SelectionMethod { get; set; } = "PromotionOrLateral";
    public bool DemoteOverRanked { get; set; } = false;
    public bool AutoPromoteInRankRange { get; set; } = false;
    public int? SupervisorPositionBlueprintId { get; set; }
    public int ZIndex { get; set; } = 0;
}
