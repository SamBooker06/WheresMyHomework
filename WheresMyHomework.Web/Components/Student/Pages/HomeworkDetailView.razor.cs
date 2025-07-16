using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WheresMyHomework.Core.Services.Auth;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Core.Services.SubjectService;
using WheresMyHomework.Core.Services.TagService;
using WheresMyHomework.Core.Services.Users;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Web.Components.Student.Pages;

public partial class HomeworkDetailView
{
    // This is how often the notes will try to update itself on the server when modified
    // If another modification happens within this interval, the timer will reset
    private const int NoteUpdateInterval = 500;

    [Parameter, EditorRequired] public required int HomeworkId { get; init; }

    private StudentHomeworkResponseInfo? homeworkInfo;
    private UserInfo? userInfo;
    private TeacherInfo? teacherInfo;
    private SubjectResponseInfo? subjectInfo;

    // Maps a given priority to its relevant CSS styling
    private Dictionary<Priority, string> priorityCssMap = new()
    {
        [Priority.Low] = "bg-success",
        [Priority.Medium] = "bg-warning",
        [Priority.High] = "bg-danger",
        [Priority.None] = "text-muted border"
    };


    private bool isComplete;

    // Todos
    private List<TodoResponseInfo> todos = [];
    private int? editedTodoId;
    private int? mousedOverTodoId;
    private InputText? newTodoInput;
    private bool shouldFocusOnNewTodo;

    // Notes
    private Timer? notesAutoSaveTimer;
    private bool noteIsUpdating;

    // Tags
    private List<TagResponseInfo> tags = [];
    private string? mousedOverTagName;

    private HomeworkModel homeworkModel = new();

    protected override async Task OnInitializedAsync()
    {
        var authInfo = await StudentAuthService.GetAuthenticatedUserInfoAsync();
        if (authInfo is null) return;

        // Load relevant info
        await LoadDataAsync(authInfo);
        if (homeworkInfo is null) return;

        // Set the fields with the loaded data
        isComplete = homeworkInfo.IsComplete;
        todos = [..homeworkInfo.Todos];
        tags = [..homeworkInfo.Tags];

        InitialiseModel();
    }

    private async Task LoadDataAsync(AuthInfo authInfo)
    {
        homeworkInfo = await HomeworkService.GetStudentHomeworkInfoByIdAsync(HomeworkId, authInfo.UserId);
        teacherInfo = await TeacherService.GetTeacherByHomeworkIdAsync(HomeworkId);
        subjectInfo = await SubjectService.GetSubjectInfoAsync(homeworkInfo.Class.SubjectId);
        userInfo = await StudentService.GetStudentInfoAsync(authInfo.UserId);
    }

    private void InitialiseModel()
    {
        if (homeworkInfo is null) return;

        homeworkModel.NotesDescription = homeworkInfo.Notes;
        homeworkModel.Priority = homeworkInfo.Priority;
    }

    private async Task UpdateTodoStatusAsync(int todoId, bool newCompletionStatus)
    {
        await TodoService.UpdateTodoStatusAsync(todoId, newCompletionStatus);
    }

    private async Task AddNewTodoAsync()
    {
        if (homeworkInfo is null) return;

        var todoRequest = new TodoRequestInfo
        {
            StudentHomeworkTaskId = homeworkInfo.StudentHomeworkId,
            Description = homeworkModel.NewTodoDescription,
            IsComplete = false
        };

        var todoResponse = await TodoService.CreateNewTodoAsync(todoRequest);
        if (todoResponse is null) return;

        todos.Add(todoResponse);
        homeworkModel.NewTodoDescription = string.Empty; // Clear form data
        
        shouldFocusOnNewTodo = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender && shouldFocusOnNewTodo) await FocusOnNewTodoInputFieldAsync();
    }

    private async Task FocusOnNewTodoInputFieldAsync()
    {
        if (newTodoInput is null) return;
        if (newTodoInput.Element.HasValue) await newTodoInput.Element.Value.FocusAsync();

        shouldFocusOnNewTodo = false;
    }

    private async Task UpdateNotesAsync(string newNote)
    {
        if (homeworkInfo is null) return;

        noteIsUpdating = true;
        
        // If there is already a timer, stop it
        if (notesAutoSaveTimer is not null) await notesAutoSaveTimer.DisposeAsync();

        notesAutoSaveTimer = new Timer(async void (_) =>
            {
                try
                {
                    await HomeworkService.UpdateNotesAsync(homeworkInfo.StudentHomeworkId, newNote);
                    noteIsUpdating = false;
                    
                    // InvokeAsync() is needed since Blazor renders everything on a single thread, so StateHasChanged()
                    // Can only be called on the render thread, which InvokeAsync will do
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception e)
                {
                    Logger.LogError("There was a problem trying to save the changes to the notes. Error {Error}", e);
                }
            },
            null, NoteUpdateInterval, Timeout.Infinite);
    }

    private async Task ToggleCompletionStatusAsync()
    {
        if (homeworkInfo is null) return;

        isComplete = !isComplete;
        await HomeworkService.UpdateHomeworkCompletionStatusAsync(homeworkInfo.StudentHomeworkId, isComplete);
    }

    private async Task UpdateHomeworkPriorityAsync()
    {
        if (homeworkInfo is null) return;

        await HomeworkService.UpdateHomeworkPriorityAsync(homeworkInfo.StudentHomeworkId, homeworkModel.Priority);
    }

    private async Task UpdateTodoDescriptionAsync()
    {
        if (!editedTodoId.HasValue || homeworkModel.EditedTodoDescription is null) return;

        var updatedSuccessfully =
            await TodoService.UpdateTodoDescriptionAsync(editedTodoId.Value, homeworkModel.EditedTodoDescription);
        if (!updatedSuccessfully) return;

        // This may seem over the top, but since the todo responses are designed to be immutable (since they represent
        // a response from the database), the previous todo needs to be replaced with an updated copy
        var todo = todos.First(todo => todo.Id == editedTodoId);
        var todoIndex = todos.IndexOf(todo);
        var updatedTodo = todo with { Description = homeworkModel.EditedTodoDescription };

        todos.RemoveAt(todoIndex);
        todos.Insert(todoIndex, updatedTodo);

        homeworkModel.EditedTodoDescription = null;
        editedTodoId = null;
    }

    private async Task AddTagAsync()
    {
        if (homeworkInfo is null || homeworkModel.NewTagName is null) return;

        var createdTag = await TagService.AddTagAsync(new TagRequestInfo
        {
            Name = homeworkModel.NewTagName,
            StudentHomeworkId = homeworkInfo.StudentHomeworkId,
        });

        if (createdTag is null || tags.Any(tag => tag.Name == createdTag.Name)) return;

        // Update the student's view
        tags.Add(createdTag);

        // Reset the new tag input field
        homeworkModel.NewTagName = string.Empty;
    }

    private async Task DeleteTagAsync(string tagName)
    {
        if (homeworkInfo is null) return;

        var success = await TagService.DeleteTagAsync(homeworkInfo.StudentHomeworkId, tagName);
        if(!success) return;
        
        var tag = tags.FirstOrDefault(tag => tag.Name == tagName);
        if (tag is null) return;

        tags.Remove(tag);
    }

    private sealed class HomeworkModel
    {
        public Priority Priority { get; set; }

        public string? EditedTodoDescription { get; set; }

        [Required]
        [MinLength(3), MaxLength(35)]
        public string NewTodoDescription { get; set; } = string.Empty;

        public string NotesDescription { get; set; } = string.Empty;

        public string? NewTagName { get; set; }
    }
}