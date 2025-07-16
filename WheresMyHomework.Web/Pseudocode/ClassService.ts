class Class {
    public ClassId: number;
    public Name: string;
    public StudentIds: string[];
}

class ClassService {
    public GetClassById(classId: string): Class {}
    public CreateClass(cls: Class) {}
    
    public AddStudentToClass(classId: number, studentId: string) {}
    public RemoveStudentFromClass(classId: number, studentId: string) {}
}