using MC_Checker.Enums;

namespace MC_Checker
{
    public class MCAccount
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsValid { get; set; }

        public bool IsPremium { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsEmailVerified { get; set; }

        public bool IsSecretQuestion { get; set; }
        public bool HasGifts { get; set; }

        public AuthType AuthType;

        public MCAccount(string Email, string Password)
        {
            this.Email = Email;
            this.Password = Password;
            IsValid = false;
        }

        public MCAccount(string Name, string Email, string Password, bool IsPremium, bool IsSuspended, bool IsBlocked, bool IsEmailVerified)
        {
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;

            this.IsPremium = IsPremium;
            this.IsSuspended = IsSuspended;
            this.IsBlocked = IsBlocked;
            this.IsEmailVerified = IsEmailVerified;

            AuthType = AuthType.CLIENT;
            IsValid = true;
        }

        public MCAccount(string Name, string Email, string Password, bool IsPremium, bool IsSecretQuestion, bool HasGifts)
        {
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;

            this.IsPremium = IsPremium;
            this.IsSecretQuestion = IsSecretQuestion;
            this.HasGifts = HasGifts;

            AuthType = AuthType.SITE;
            IsValid = true;
        }
    }
}
