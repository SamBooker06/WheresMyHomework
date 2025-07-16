enum PersonTitle {
    Mr,
    Mrs,
    Dr,
    Miss
}

enum UserType {
    Admin,
    Teacher,
    Student
}

interface UserInfo {
    UserId: string;
    FirstName: string;
    LastName: string;
    SchoolId: number;
    Title: PersonTitle;
    UserType: UserType;
}
class User {}

class UserService {
    public GetUserInfo(userId: string): UserInfo{}
    
    public CreateUser(userInfo: UserInfo): User {}
}