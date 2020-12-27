using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Data
{
    public class IdentityServerDb:IdentityDbContext<IdentityServerUser, IdentityRole<Guid>,Guid>
    {
        public IdentityServerDb(DbContextOptions<IdentityServerDb> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");
            builder.Entity<IdentityServerUser>(e=> {
                e.ToTable("Users");
            });
            builder.Entity<IdentityRole<Guid>>(e => {
                e.ToTable("Roles");
            });
            builder.Entity<IdentityUserClaim<Guid>>(e => {
                e.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<Guid>>(e => {
                e.HasKey(x => new { x.UserId});
                e.ToTable("UserLogins");
            });
            builder.Entity<IdentityUserRole<Guid>>(e => {
                e.HasKey(x => new { x.UserId,x.RoleId });
                e.ToTable("UsersRoles");
            });
            builder.Entity<IdentityUserToken<Guid>>(e => {
                e.HasKey(x => new { x.UserId});
                e.ToTable("UserTokens");
            });
            builder.Entity<IdentityRoleClaim<Guid>>(e => {
                e.HasKey(x => new { x.RoleId });
                e.ToTable("RoleClaims");
            });
           
        }
    }
}
