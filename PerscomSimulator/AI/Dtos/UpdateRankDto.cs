namespace Perscom.AI.Dtos;

public class UpdateRankDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public int? RankClassificationId { get; set; }
    public int? Precedence { get; set; }
    public bool? IsPositional { get; set; }
    public int? NextRankId { get; set; }
    public bool ClearNextRankId { get; set; } = false;
    public string Image { get; set; }
}