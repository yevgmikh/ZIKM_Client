namespace ZIKM_Client.Infrastructure
{
    public struct LoginData
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }

        public LoginData(string user, string password, string captcha)
        {
            User = user;
            Password = password;
            Captcha = captcha;
        }
    }
}
