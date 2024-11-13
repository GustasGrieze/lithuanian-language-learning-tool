using Microsoft.AspNetCore.Components.Authorization;
using lithuanian_language_learning_tool.Models;
using System.Security.Claims;
using lithuanian_language_learning_tool.Data;
using Microsoft.EntityFrameworkCore;

namespace lithuanian_language_learning_tool.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserFromGoogleAsync(AuthenticationState authState);
        Task<User?> GetCurrentUserAsync(AuthenticationState authState);
        User CreateGuestUser();
        Task<bool> PromoteToAdminAsync(string userId);
        Task<bool> DemoteFromAdminAsync(string userId);

        Task UpdateUserAsync(User user);

    }

    public class UserService : IUserService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly HashSet<string> _adminEmails = new()
        {
            "3darijus@gmail.com",
            "testporator@gmail.com"
        };

        public UserService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User> GetOrCreateUserFromGoogleAsync(AuthenticationState authState)
        {
            using var context = _contextFactory.CreateDbContext();

            var userClaims = authState.User;

            if (!userClaims.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var googleId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = userClaims.FindFirst(ClaimTypes.Email)?.Value;
            var givenName = userClaims.FindFirst(ClaimTypes.GivenName)?.Value;
            var familyName = userClaims.FindFirst(ClaimTypes.Surname)?.Value;
            var name = userClaims.FindFirst(ClaimTypes.Name)?.Value;
            var profilePictureUrl = userClaims.FindFirst("picture")?.Value;

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

            if (existingUser != null)
            {
                existingUser.LastLoginAt = DateTime.UtcNow;
                await context.SaveChangesAsync(); // Update last login time
                return existingUser;
            }

            var newUser = new User
            {
                GoogleId = googleId,
                Email = email,
                GivenName = givenName,
                FamilyName = familyName,
                DisplayName = name,
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ProfilePictureUrl = profilePictureUrl,
                Role = _adminEmails.Contains(email) ? UserRole.Admin : UserRole.RegisteredUser
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            return newUser;
        }

        public User CreateGuestUser()
        {
            // edit: no adding Guest user to DB

            //using var context = _contextFactory.CreateDbContext();
            var guestUser = new User
            {
                DisplayName = "Guest",
                Role = UserRole.Guest,
                Email = "",
                LastLoginAt = DateTime.UtcNow
            };

            //context.Users.Add(guestUser);
            //context.SaveChanges(); // Note: This is synchronous; for async, use SaveChangesAsync in async contexts
            return guestUser;
        }

        public async Task<User?> GetCurrentUserAsync(AuthenticationState authState)
        {
            using var context = _contextFactory.CreateDbContext();
            var userClaims = authState.User;

            if (!userClaims.Identity?.IsAuthenticated ?? true)
            {
                return CreateGuestUser();
            }

            var googleId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<bool> PromoteToAdminAsync(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.Role = UserRole.Admin;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DemoteFromAdminAsync(string userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.Role = UserRole.RegisteredUser;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateUserAsync(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
