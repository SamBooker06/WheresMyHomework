using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Core.Services.SubjectService;
using WheresMyHomework.Core.Services.TagService;
using WheresMyHomework.Core.Services.Users;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Web.Components.Student.Pages;

//TODO: Move into a teacher and student HomeworkDetailView
public partial class HomeworkDetailView
{
    private const int NoteUpdateInterval = 500;

    [Parameter, EditorRequired] public int HomeworkId { get; init; }

    private StudentHomeworkResponseInfo _homeworkInfo = null!;
    private UserInfo _userInfo;
    private TeacherInfo _teacherInfo = null!;
    private SubjectResponseInfo _subjectInfo = null!;

    private Dictionary<Priority, string> _priorityCssMap = new()
    {
        [Priority.Low] = "bg-success",
        [Priority.Medium] = "bg-warning",
        [Priority.High] = "bg-danger",
        [Priority.None] = "text-muted border"
    };


    private bool _isComplete;

    private IList<TodoResponseInfo> _todos = null!;
    private int? _editedTodoId { get; set; }
    private int? _mousedOverTodoId;

    private Timer? _notesAutoSaveTimer;
    private bool _noteIsUpdating;
    private InputText? _newTodoInput;
    private bool _shouldFocusOnNewTodo;

    private IList<TagResponseInfo> _tags = [];

    private string? _mousedOverTagName;
    private string? mousedOverTagName
    {
        get => _mousedOverTagName;
        set
        {
            _mousedOverTagName = value;
            StateHasChanged();
        }
    }

    private HomeworkModel _homeworkModel = new();

    protected override async Task OnInitializedAsync()
    {
        var student = await StudentAuthService.GetAuthenticatedUserInfoAsync();
        Debug.Assert(student != null);

        _homeworkInfo = await HomeworkService.GetStudentHomeworkInfoByIdAsync(HomeworkId, student.UserId);
        _teacherInfo = await TeacherService.GetTeacherByHomeworkIdAsync(HomeworkId);
        _subjectInfo = await SubjectService.GetSubjectInfoAsync(_homeworkInfo.Class.SubjectId);
        _todos = new List<TodoResponseInfo>(_homeworkInfo.Todos);
        _userInfo = await StudentService.GetStudentInfoAsync(student.UserId);
        _isComplete = _homeworkInfo.IsComplete;

        _tags = new List<TagResponseInfo>(_homeworkInfo.Tags);
        
        _homeworkModel.NotesDescription = _homeworkInfo.Notes;
        _homeworkModel.Priority = _homeworkInfo.Priority;
    }

    private async Task UpdateTodoStatusAsync(int todoId, bool isComplete)
    {
        await TodoService.UpdateTodoStatusAsync(todoId, isComplete);
    }

    private async Task AddNewTodo()
    {
        var todoRequest = new TodoRequestInfo
        {
            StudentHomeworkTaskId = _homeworkInfo.StudentHomeworkId,
            Description = _homeworkModel.NewTodoDescription,
            IsComplete = false
        };

        var todoResponse = await TodoService.CreateNewTodoAsync(todoRequest);

        // Clear post data
        Navigator.NavigateTo($"Homework/{HomeworkId}");

        _todos.Add(todoResponse);
        _homeworkModel.NewTodoDescription = string.Empty; // Clear form data
        _shouldFocusOnNewTodo = true;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocusOnNewTodo && _newTodoInput.Element.HasValue)
            await _newTodoInput.Element!.Value.FocusAsync();
        _shouldFocusOnNewTodo = false;
    }

    private async Task OnNoteUpdate(string newNote)
    {
        _noteIsUpdating = true;
        if (_notesAutoSaveTimer is not null) await _notesAutoSaveTimer.DisposeAsync();

        _notesAutoSaveTimer = new Timer(async void (_) =>
            {
                try
                {
                    await HomeworkService.UpdateNotesAsync(_homeworkInfo.StudentHomeworkId, newNote);
                    _noteIsUpdating = false;
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            },
            null, NoteUpdateInterval, Timeout.Infinite);
    }

    private async Task ToggleCompletionStatusAsync()
    {
        _isComplete = !_isComplete;
        await HomeworkService.UpdateHomeworkCompletionStatusAsync(_homeworkInfo.StudentHomeworkId, _isComplete);
        StateHasChanged();
    }

    private async Task OnPriorityUpdatedAsync()
    {
        await HomeworkService.UpdateHomeworkPriorityAsync(_homeworkInfo.StudentHomeworkId, _homeworkModel.Priority);
        StateHasChanged();
    }

    private sealed class HomeworkModel
    {
        [Required]
        [MinLength(3), MaxLength(35)]
        public string NewTodoDescription { get; set; } = string.Empty;

        public string NotesDescription { get; set; } = string.Empty;

        public Priority Priority { get; set; }
        public string? EditedTodoDescription { get; set; }
        public string? NewTagName { get; set; }
    }

    private async Task UpdateTodoDescriptionAsync()
    {
        Console.WriteLine("Update");
        if (_editedTodoId is null || _homeworkModel.EditedTodoDescription is null) return;
        if (await TodoService.UpdateTodoDescriptionAsync((int)_editedTodoId, _homeworkModel.EditedTodoDescription) is false) return;
        
        var oldTodo = _todos.First(todo => todo.Id == _editedTodoId);
        var todoIndex = _todos.IndexOf(oldTodo);
        var updatedTodo = oldTodo with { Description = _homeworkModel.EditedTodoDescription};
        
        _todos.RemoveAt(todoIndex);
        _todos.Insert(todoIndex, updatedTodo);
        _homeworkModel.EditedTodoDescription = null;
        _editedTodoId = null;
        
        StateHasChanged();
    }

    private async Task AddTagAsync()
    {
        var tagResponse = await TagService.AddTagAsync(new TagRequestInfo
        {
            Name = _homeworkModel.NewTagName,
            StudentHomeworkId = _homeworkInfo.StudentHomeworkId,
        });
        
        if (tagResponse is null || _tags.Any(tag => tag.Name == tagResponse.Name)) return;
        _tags.Add(tagResponse);
        
        _homeworkModel.NewTagName = string.Empty;
    }

    private async Task DeleteTagAsync(string tagName)
    {
        await TagService.DeleteTagAsync(_homeworkInfo.StudentHomeworkId, tagName);
        var tag = _tags.FirstOrDefault(tag => tag.Name == tagName);
        if (tag is null) return;
        
        _tags.Remove(tag);
        _homeworkModel.NewTagName = string.Empty;
    }
}