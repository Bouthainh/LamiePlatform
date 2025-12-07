namespace BadeePlatform.Helpers
{
    public static class ChildExtensions
    {
        public static string GetIconPathByGender(string gender)
        {
            return gender == "ذكر"
                ? "/images/ChildBoy.png"
                : "/images/ChildGirl.png";
        }
    }
}
