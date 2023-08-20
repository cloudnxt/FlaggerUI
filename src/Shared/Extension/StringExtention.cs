namespace Gates.Shared.Extension
{
    public static class StringExtension
    {
        public static bool IsFlaggerResource(this string appname)
        {
            return appname.EndsWith("-primary") && appname.EndsWith("-canary");
        }
    }
}
