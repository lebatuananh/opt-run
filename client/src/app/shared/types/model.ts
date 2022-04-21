export class User {
  constructor(accessToken: string, username: string, fullName: string, email: string, avatar: string, role: string
  ) {
    this.accessToken = accessToken;
    this.fullName = fullName;
    this.username = username;
    this.email = email;
    this.avatar = avatar;
    this.role = role;
  }
  public id: string;
  public accessToken: string;
  public username: string;
  public fullName: string;
  public email: string;
  public avatar: string;
  public role: string;
}
