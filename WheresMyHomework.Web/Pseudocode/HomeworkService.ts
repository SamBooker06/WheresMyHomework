class HomeworkTask {
    HomeworkId: number;
    Title: string;
    TeacherId: string;
    Description: string;
    SetDate: Date;
    DueDate: Date;
}

class Todo {
    TodoId: number;
    Description: string;
    IsComplete: boolean;
}

class HomeworkService {
    public GetHomeworkById(homeworkId: number): HomeworkTask {}
    public GetTasksForClass(classId: number): HomeworkTask[] {}
    public GetTasksForStudent(studentId: string): HomeworkTask[] {}
    
    public CreateHomework(homeworkTask: HomeworkTask): number {}
    
    public DeleteHomework(homeworkId: number): void {}
    
    public UpdateDescription(homeworkId: number, description: string): void {}
    
    public UpdateCompletionStatus(homeworkId: number, studentId: string, isComplete: boolean): void {}
    public AddTodo(homeworkId: number, studentId: string, todoDescription: string): number {}
    public DeleteTodo(todoId: number): void {}
    public UpdateTodoStatus(todoId: number, isComplete: boolean): void {}
    public GetTodos(homeworkId: number, userId: string): Todo[] {}
    
}