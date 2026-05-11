namespace Perscom.AI.Dtos;

public class RankDto
{
    public int RankClassificationId { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public int Precedence { get; set; } = 0;
    public bool IsPositional { get; set; } = false;
    public int? NextRankId { get; set; }
    public string Image { get; set; } = "";
}