using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Homework.DTO;

public class StudentHomeworkFilter
{
    public static readonly StudentHomeworkFilter Empty = new StudentHomeworkFilter();

    public string? Title { get; set; }
    public bool ExactMatch { get; set; }

    public ICollection<Priority> Priorities { get; set; } = [];
    public ICollection<string> Tags { get; set; } = [];
}
