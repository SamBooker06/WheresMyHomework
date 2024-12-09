using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Homework.DTO;

public record StudentHomeworkFilter
{
    public static readonly StudentHomeworkFilter Empty = new StudentHomeworkFilter();

    public string? Title { get; set; }
    public bool ExactMatch { get; set; } = false;

    public ICollection<Priority> Priorities { get; set; } = [];
}