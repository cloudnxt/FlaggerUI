namespace Gates.Shared.Extension
{
    public static class StringExtension
    {
        public static bool IsFlaggerResource(this string appname)
        {
           if(appname.EndsWith("-primary") || appname.EndsWith("-canary"))
                return true;
           return false;
        }
    }
}
