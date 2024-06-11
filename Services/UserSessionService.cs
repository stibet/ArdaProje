using Microsoft.AspNetCore.Http;

namespace ArdaProje.Services
{
    public interface IUserSessionService
    {
        int? UserId { get; set; }  
        int? PtID { get; set; }
        string Username { get; set; }
    }

    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? PtID
        {
            get => _httpContextAccessor.HttpContext.Session.GetInt32("PtID");
            set
            {
                if (value.HasValue)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("PtID", value.Value);
                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.Remove("PtID");  // PtID'yi session'dan kaldır
                }
            }
        }

        public int? UserId  // Nullable int yapıldı
        {
            get => _httpContextAccessor.HttpContext.Session.GetInt32("UserID");
            set
            {
                if (value.HasValue)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("UserID", value.Value);
                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.Remove("UserID");  // UserID'yi session'dan kaldır
                }
            }
        }

        public string Username
        {
            get => _httpContextAccessor.HttpContext.Session.GetString("Username");
            set => _httpContextAccessor.HttpContext.Session.SetString("Username", value ?? string.Empty);  // Boş olursa, boş string atayın
        }
    }
}
