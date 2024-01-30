namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        public string OldPassword{ get; set; }
        public string NewPassword{ get; set; }
        public string ConfirmPassword{ get; set; }
    }
}
