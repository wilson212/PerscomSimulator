namespace Perscom.AI.Dtos;

public class UpdateRankClassificationDto
{
    public int Id { get; set; }
    // Type and PayGrade are NOT updatable — they form the composite unique key.
    public string Selection { get; set; }
    public int? LockInTime { get; set; }
    public int? MinTimeInGrade { get; set; }
    public int? MaxTimeInGrade { get; set; }
    public int? PreviousTimeInGradeRequirement { get; set; }
    public int? PromotableLength { get; set; }
    public double? Stipend { get; set; }
    public bool? HasSplitRankLanes { get; set; }
}