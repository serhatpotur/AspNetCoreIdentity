namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
                
        }
        public SignInViewModel(string? userName, string? password, bool rememberMe)
        {
            UserName = userName;
            Password = password;
            RememberMe = rememberMe;
        }

      

        public string? UserName { get; set; }
        public string? Password{ get; set; }
        public bool RememberMe { get; set; }
    }
}
