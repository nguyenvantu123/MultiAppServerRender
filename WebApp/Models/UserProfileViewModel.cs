namespace WebApp.Models
{
    public class UserProfileViewModel
    {

        public string LastPageVisited { get; set; }

        public bool IsNavOpen { get; set; }

        public bool IsNavMinified { get; set; }

        public int Count { get; set; }

        public bool IsDarkMode { get; set; }

        public UserProfileViewModel()
        {
            LastPageVisited = "/";
            IsNavOpen = true;
            IsNavMinified = false;
            Count = 0;
            IsDarkMode = false;
        }
    }
}
