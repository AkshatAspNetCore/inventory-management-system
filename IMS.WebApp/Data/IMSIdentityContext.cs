using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IMS.WebApp.Data
{
    public class IMSIdentityContext(DbContextOptions<IMSIdentityContext> options) : IdentityDbContext<IdentityUser>(options)
    {
    }
}
