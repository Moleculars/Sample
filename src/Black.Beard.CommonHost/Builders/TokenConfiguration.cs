using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Security.Jwt;

namespace Bb.Builders
{

    [ExposeClass(Context = ConstantsCore.Configuration, Filename = "TokenConfiguration", ExposedType =typeof(JwtTokenConfiguration), LifeCycle = IocScopeEnum.Singleton )]
    public class TokenConfiguration : JwtTokenConfiguration
    {

        public TokenConfiguration()
        {

        }

    }


}
