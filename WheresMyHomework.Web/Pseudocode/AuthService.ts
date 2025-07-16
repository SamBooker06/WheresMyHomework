import {AuthInfo} from "./AuthInfo";

class AuthService {
    SignIn(email: string, password: string): AuthInfo{}
    IsSignedIn(): boolean {}
    IsStudent(): boolean {}
    IsTeacher(): boolean {}
    IsAdmin(): boolean {}
    
    GetAuthInfo(): AuthInfo {}
}