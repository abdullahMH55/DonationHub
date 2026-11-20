using DonationHub.Application.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Identity;

public class IdentitySeeder
{
    private readonly SeedAdminOptions _adminOptions;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly TimeProvider _timeProvider;

    public IdentitySeeder(
        IOptions<SeedAdminOptions> adminOptions, RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager,TimeProvider timeProvider)
    {
        _adminOptions = adminOptions.Value;
        _userManager = userManager;
        _roleManager = roleManager;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var roles = new List<string>{ "Admin", "CampaignManager", "Donor" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        if (await _userManager.FindByEmailAsync(_adminOptions.Email) == null)
        {
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = _adminOptions.Email,
                Email = _adminOptions.Email,
                FullName = _adminOptions.FullName,
                IsActive = true,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
            };

            if ((await _userManager.CreateAsync(adminUser, _adminOptions.Password)).Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
