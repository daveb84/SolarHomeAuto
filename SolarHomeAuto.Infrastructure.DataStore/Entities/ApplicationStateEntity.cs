using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class ApplicationStateEntity : ApplicationState
    {
        public int Id { get; set; }

        public ApplicationState ToDomain()
        {
            return SimpleMapper.MapToSuperClass<ApplicationStateEntity, ApplicationState>(this);
        }

        public static ApplicationStateEntity FromDomain(ApplicationState data)
        {
            return SimpleMapper.MapToSubClass<ApplicationState, ApplicationStateEntity>(data);
        }

        public void CopyFrom(ApplicationState data)
        {
            SimpleMapper.MapToSubClass(data, this);
        }
    }
}
