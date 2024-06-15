using KnotBond.Entities;

namespace KnotBond.interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);

}
