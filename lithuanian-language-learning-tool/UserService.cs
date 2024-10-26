using lithuanian_language_learning_tool.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace lithuanian_language_learning_tool.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserFromGoogleAsync(AuthenticationState authState);
        Task<User?> GetCurrentUserAsync(AuthenticationState authState);
        User CreateGuestUser();
        Task<bool> PromoteToAdminAsync(string userId);
        Task<bool> DemoteFromAdminAsync(string userId);

    }

    public class UserService : IUserService
    {
        private readonly List<User> _users = new(); // To be replaced with your actual database context
        private readonly HashSet<string> _adminEmails = new()
        {
            // Add admin emails here
            "3darijus@gmail.com"
        };

        public async Task<User> GetOrCreateUserFromGoogleAsync(AuthenticationState authState)
        {
            var user = authState.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var googleId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var givenName = user.FindFirst(ClaimTypes.GivenName)?.Value;
            var familyName = user.FindFirst(ClaimTypes.Surname)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
           
            var existingUser = _users.FirstOrDefault(u => u.GoogleId == googleId);
            
            if (existingUser != null)
            {
                existingUser.LastLoginAt = DateTime.UtcNow;
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
                ProfilePictureUrl = user.FindFirst("picture")?.Value,
                Role = _adminEmails.Contains(email) ? UserRole.Admin : UserRole.RegisteredUser
            };

            _users.Add(newUser);
            return newUser;
        }

        public User CreateGuestUser()
        {
            var guestUser = new User
            {
                DisplayName = "Guest",
                Role = UserRole.Guest,
                Email = "",
                LastLoginAt = DateTime.UtcNow
            };

            _users.Add(guestUser);
            return guestUser;
        }

        public async Task<User?> GetCurrentUserAsync(AuthenticationState authState)
        {
            var user = authState.User;
            
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return CreateGuestUser();
            }

            var googleId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _users.FirstOrDefault(u => u.GoogleId == googleId);
        }

        public async Task<bool> PromoteToAdminAsync(string userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;
            user.Role = UserRole.Admin;
            return true;
        }

        public async Task<bool> DemoteFromAdminAsync(string userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;
            user.Role = UserRole.RegisteredUser;
            return true;
        }
    }
}
