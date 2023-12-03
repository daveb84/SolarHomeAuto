using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class AuthTokenEntity : AccountOAuthToken
    {
        public int Id { get; set; }

        public void Update(AccountOAuthToken data)
        {
            SimpleMapper.MapToSubClass(data, this);
        }

        public static AuthTokenEntity FromDomain(AccountOAuthToken data)
        {
            return SimpleMapper.MapToSubClass<AccountOAuthToken, AuthTokenEntity>(data);
        }

        public AccountOAuthToken ToDomain()
        {
            return SimpleMapper.MapToSuperClass<AuthTokenEntity, AccountOAuthToken>(this);
        }
    }
}
