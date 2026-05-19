namespace Perscom.AI.Dtos;

public class UpdatePositionBlueprintDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? UnitBlueprintId { get; set; }
    public int? CatagoryId { get; set; }
    public int? TargetRankId { get; set; }
    public int? PositionalRankId { get; set; }
    public bool ClearPositionalRankId { get; set; } = false;
    public string Flag { get; set; }
    public int? PromotionEchelonId { get; set; }
    public int? OccupationId { get; set; }
    public int? Stature { get; set; }
    public int? Prestige { get; set; }
    public int? MinTourLength { get; set; }
    public int? MaxTourLength { get; set; }
    public bool? CanRetireEarly { get; set; }
    public bool? CanBePromotedEarly { get; set; }
    public bool? CanLateralEarly { get; set; }
    public bool? Waiverable { get; set; }
    public string SelectionMethod { get; set; }
    public bool? DemoteOverRanked { get; set; }
    public bool? AutoPromoteInRankRange { get; set; }
    public int? SupervisorPositionBlueprintId { get; set; }
    public bool ClearSupervisorId { get; set; } = false;
    public int? ZIndex { get; set; }
}