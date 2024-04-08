using System.Collections.Concurrent;

namespace AnimaApi
{
    public class AuthenticationModels
    {
        public enum UserRoles { Admin, Player}
        public class UserModel
        {
            //public string Username { get; set; }
            public string Password { get; set; }

            public int UserId { get; set; }
            public UserRoles Role { get; set; }
        }

        public class UserLogin
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class UserConstants
        {
            public static ConcurrentDictionary<string, UserModel> Users = new()
            {
                ["string"] = new UserModel() { Password = "string", UserId = 0, Role = UserRoles.Admin },
                ["bob"] = new UserModel(){ Password = "password", UserId = 1, Role = UserRoles.Player }
            };
        }
    }
}
