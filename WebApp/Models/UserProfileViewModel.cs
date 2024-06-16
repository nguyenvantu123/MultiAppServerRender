namespace WebApp.Models
{
    public class UserProfile
    {

        public string LastPageVisited { get; set; }

        public bool IsNavOpen { get; set; }

        public bool IsNavMinified { get; set; }

        public int Count { get; set; }

        public UserProfile()
        {
            LastPageVisited = "/";
            IsNavOpen = true;
            IsNavMinified = false;
            Count = 0;
        }
    }
}
