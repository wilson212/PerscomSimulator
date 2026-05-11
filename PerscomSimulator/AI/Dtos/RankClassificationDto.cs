namespace Perscom.AI.Dtos;

public class RankClassificationDto
{
    public string Type { get; set; }              // "Enlisted", "Officer", "Warrant"
    public int PayGrade { get; set; }
    public string Selection { get; set; } = "Automatic"; // "EntryLevel", "Automatic", "PromotionBoard", "SelectionProcedure"
    public int LockInTime { get; set; } = 0;
    public int MinTimeInGrade { get; set; } = 0;
    public int MaxTimeInGrade { get; set; } = 0;
    public int PreviousTimeInGradeRequirement { get; set; } = 12;
    public int PromotableLength { get; set; } = 12;
    public double Stipend { get; set; } = 0;
    public bool HasSplitRankLanes { get; set; } = false;
}