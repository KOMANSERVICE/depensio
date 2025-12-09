using depensio.Shared.Services;

namespace depensio.Web.Services
{
    public class WebFormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "Web";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
