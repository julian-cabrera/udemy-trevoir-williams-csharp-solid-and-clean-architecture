using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.LeaveManagement.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "75923728-ae2c-4c74-a9e4-eaad85103959",
                    UserId = "01543bbf-36fd-4bd3-867f-7d0456e06a32"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "cf4427a8-edf8-4154-8798-10176bc66a4d",
                    UserId = "3c941751-8f7d-4843-98ce-28c508e35b4d"
                });
        }
    }
}
