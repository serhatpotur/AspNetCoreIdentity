namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignUpViewModel
    {


        public SignUpViewModel()
        {
                
        }
        public SignUpViewModel(string userName, string email, string phoneNumber, string password, string confirmPassword)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
