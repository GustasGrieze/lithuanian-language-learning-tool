using lithuanian_language_learning_tool.Data;
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TestProject.Database;


namespace TestProject.Services
{
    public class UserServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly IUserService _userService;

        public UserServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _userService = _fixture.ServiceProvider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Helper method to create an AuthenticationState with specified claims.
        /// </summary>
        /// <param name="isAuthenticated">Indicates if the user is authenticated.</param>
        /// <param name="claims">Dictionary of claim types and their values.</param>
        /// <returns>An instance of AuthenticationState.</returns>
        private AuthenticationState CreateAuthenticationState(bool isAuthenticated, Dictionary<string, string>? claims = null)
        {
            var identity = isAuthenticated ? new ClaimsIdentity("TestAuthType") : new ClaimsIdentity();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    identity.AddClaim(new Claim(claim.Key, claim.Value));
                }
            }
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        #region GetOrCreateUserFromGoogleAsync Tests

        [Fact]
        public async Task GetOrCreateUserFromGoogleAsync_CreatesNewUser_WhenUserDoesNotExist()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google123" },
                { ClaimTypes.Email, "newuser@example.com" },
                { ClaimTypes.GivenName, "New" },
                { ClaimTypes.Surname, "User" },
                { ClaimTypes.Name, "New User" },
                { "picture", "http://example.com/picture.jpg" }
            };
            var authState = CreateAuthenticationState(true, claims);

            // Act
            var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("google123", user.GoogleId);
            Assert.Equal("newuser@example.com", user.Email);
            Assert.Equal("New", user.GivenName);
            Assert.Equal("User", user.FamilyName);
            Assert.Equal("New User", user.DisplayName);
            Assert.Equal("http://example.com/picture.jpg", user.ProfilePictureUrl);
            Assert.Equal(UserRole.RegisteredUser, user.Role);
            Assert.True(user.CreatedAt <= DateTime.UtcNow.ToLocalTime());
            Assert.True(user.LastLoginAt <= DateTime.UtcNow.ToLocalTime());
        }

        [Fact]
        public async Task GetOrCreateUserFromGoogleAsync_RetrievesExistingUser_AndUpdatesLastLogin()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var initialClaims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google456" },
                { ClaimTypes.Email, "existinguser@example.com" },
                { ClaimTypes.GivenName, "Existing" },
                { ClaimTypes.Surname, "User" },
                { ClaimTypes.Name, "Existing User" },
                { "picture", "http://example.com/existing.jpg" }
            };
            var initialAuthState = CreateAuthenticationState(true, initialClaims);

            // Create the user initially
            var initialUser = await _userService.GetOrCreateUserFromGoogleAsync(initialAuthState);

            // Wait for a second to ensure LastLoginAt changes
            await Task.Delay(1000);

            // Act
            var updatedUser = await _userService.GetOrCreateUserFromGoogleAsync(initialAuthState);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(initialUser.Id, updatedUser.Id);
            Assert.Equal(initialUser.GoogleId, updatedUser.GoogleId);
            Assert.Equal(initialUser.Email, updatedUser.Email);
            Assert.Equal(initialUser.GivenName, updatedUser.GivenName);
            Assert.Equal(initialUser.FamilyName, updatedUser.FamilyName);
            Assert.Equal(initialUser.DisplayName, updatedUser.DisplayName);
            Assert.Equal(initialUser.ProfilePictureUrl, updatedUser.ProfilePictureUrl);
            Assert.Equal(initialUser.Role, updatedUser.Role);
            Assert.True(updatedUser.LastLoginAt > initialUser.LastLoginAt);
        }

        [Fact]
        public async Task GetOrCreateUserFromGoogleAsync_ThrowsUnauthorizedAccessException_WhenUserNotAuthenticated()
        {
            // Arrange
            var authState = CreateAuthenticationState(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.GetOrCreateUserFromGoogleAsync(authState));
        }

        [Fact]
        public async Task GetOrCreateUserFromGoogleAsync_SetsRoleToAdmin_WhenEmailIsAdminEmail()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var adminEmails = new List<string>
            {
                "3darijus@gmail.com",
                "testporator@gmail.com"
            };

            foreach (var adminEmail in adminEmails)
            {
                var claims = new Dictionary<string, string>
                {
                    { ClaimTypes.NameIdentifier, Guid.NewGuid().ToString() },
                    { ClaimTypes.Email, adminEmail },
                    { ClaimTypes.GivenName, "Admin" },
                    { ClaimTypes.Surname, "User" },
                    { ClaimTypes.Name, "Admin User" },
                    { "picture", "http://example.com/admin.jpg" }
                };
                var authState = CreateAuthenticationState(true, claims);

                // Act
                var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

                // Assert
                Assert.NotNull(user);
                Assert.Equal(UserRole.Admin, user.Role);
            }
        }

        [Fact]
        public async Task GetOrCreateUserFromGoogleAsync_HandlesMissingClaimsGracefully()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                // Missing Email, GivenName, Surname, etc.
                { ClaimTypes.NameIdentifier, "googleMissingClaims" }
            };
            var authState = CreateAuthenticationState(true, claims);

            // Act
            var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("googleMissingClaims", user.GoogleId);
            Assert.Equal("unknown@example.com", user.Email); // Default Email
            Assert.Equal("Unknown", user.GivenName); // Default GivenName
            Assert.Equal("User", user.FamilyName); // Default FamilyName
            Assert.Equal("Unknown User", user.DisplayName); // Default DisplayName
            Assert.Equal(string.Empty, user.ProfilePictureUrl); // Default ProfilePictureUrl
            Assert.Equal(UserRole.RegisteredUser, user.Role);
        }

        #endregion

        #region GetCurrentUserAsync Tests

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsCurrentUser_WhenAuthenticated()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google789" },
                { ClaimTypes.Email, "currentuser@example.com" },
                { ClaimTypes.GivenName, "Current" },
                { ClaimTypes.Surname, "User" },
                { ClaimTypes.Name, "Current User" },
                { "picture", "http://example.com/current.jpg" }
            };
            var authState = CreateAuthenticationState(true, claims);

            // Create the user
            var createdUser = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Act
            var retrievedUser = await _userService.GetCurrentUserAsync(authState);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.Id, retrievedUser!.Id);
            Assert.Equal(createdUser.Email, retrievedUser.Email);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsGuestUser_WhenNotAuthenticated()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var authState = CreateAuthenticationState(false);

            // Act
            var guestUser = await _userService.GetCurrentUserAsync(authState);

            // Assert
            Assert.NotNull(guestUser);
            Assert.Equal("Guest", guestUser.DisplayName);
            Assert.Equal(UserRole.Guest, guestUser.Role);
            Assert.Equal("", guestUser.Email);
            Assert.True(guestUser.LastLoginAt <= DateTime.UtcNow);
        }

        #endregion

        #region CreateGuestUser Tests

        [Fact]
        public void CreateGuestUser_ReturnsGuestUser_WithExpectedProperties()
        {
            // Act
            var guestUser = _userService.CreateGuestUser();

            // Assert
            Assert.NotNull(guestUser);
            Assert.Equal("Guest", guestUser.DisplayName);
            Assert.Equal(UserRole.Guest, guestUser.Role);
            Assert.Equal(string.Empty, guestUser.Email);
            Assert.True(guestUser.LastLoginAt <= DateTime.UtcNow);
        }

        #endregion

        #region PromoteToAdminAsync Tests

        [Fact]
        public async Task PromoteToAdminAsync_PromotesUserToAdmin_WhenUserExists()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google321" },
                { ClaimTypes.Email, "userToPromote@example.com" },
                { ClaimTypes.GivenName, "Promote" },
                { ClaimTypes.Surname, "User" },
                { ClaimTypes.Name, "Promote User" },
                { "picture", "http://example.com/promote.jpg" }
            };
            var authState = CreateAuthenticationState(true, claims);
            var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Act
            var result = await _userService.PromoteToAdminAsync(user.Id);

            // Assert
            Assert.True(result);

            // Verify role
            var promotedUser = await _userService.GetCurrentUserAsync(authState);
            Assert.Equal(UserRole.Admin, promotedUser!.Role);
        }

        [Fact]
        public async Task PromoteToAdminAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();
            var nonExistentUserId = Guid.NewGuid().ToString();

            // Act
            var result = await _userService.PromoteToAdminAsync(nonExistentUserId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region DemoteFromAdminAsync Tests

        [Fact]
        public async Task DemoteFromAdminAsync_DemotesUserToRegisteredUser_WhenUserIsAdmin()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google654" },
                { ClaimTypes.Email, "adminToDemote@example.com" },
                { ClaimTypes.GivenName, "Demote" },
                { ClaimTypes.Surname, "Admin" },
                { ClaimTypes.Name, "Demote Admin" },
                { "picture", "http://example.com/demote.jpg" }
            };
            var authState = CreateAuthenticationState(true, claims);
            var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Promote to admin first
            var promoteResult = await _userService.PromoteToAdminAsync(user.Id);
            Assert.True(promoteResult);

            // Act
            var demoteResult = await _userService.DemoteFromAdminAsync(user.Id);

            // Assert
            Assert.True(demoteResult);

            // Verify role
            var demotedUser = await _userService.GetCurrentUserAsync(authState);
            Assert.Equal(UserRole.RegisteredUser, demotedUser!.Role);
        }

        [Fact]
        public async Task DemoteFromAdminAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();
            var nonExistentUserId = Guid.NewGuid().ToString();

            // Act
            var result = await _userService.DemoteFromAdminAsync(nonExistentUserId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetTopUsersAsync Tests

        [Fact]
        public async Task GetTopUsersAsync_ReturnsTopUsers_BasedOnHighScore()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var users = new List<User>
            {
                new User
                {
                    Email = "user1@example.com",
                    HighScore = 150,
                    GoogleId = "googleUser1",
                    DisplayName = "User 1",
                    GivenName = "User",
                    FamilyName = "One",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                },
                new User
                {
                    Email = "user2@example.com",
                    HighScore = 200,
                    GoogleId = "googleUser2",
                    DisplayName = "User 2",
                    GivenName = "User",
                    FamilyName = "Two",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                },
                new User
                {
                    Email = "user3@example.com",
                    HighScore = 100,
                    GoogleId = "googleUser3",
                    DisplayName = "User 3",
                    GivenName = "User",
                    FamilyName = "Three",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                },
                new User
                {
                    Email = "user4@example.com",
                    HighScore = 250,
                    GoogleId = "googleUser4",
                    DisplayName = "User 4",
                    GivenName = "User",
                    FamilyName = "Four",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                },
                new User
                {
                    Email = "user5@example.com",
                    HighScore = 50,
                    GoogleId = "googleUser5",
                    DisplayName = "User 5",
                    GivenName = "User",
                    FamilyName = "Five",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                }
            };

            // Create a new scope for resolving AppDbContext
            using (var scope = _fixture.ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Act
            var top3Users = await _userService.GetTopUsersAsync(3);

            // Assert
            Assert.NotNull(top3Users);
            Assert.Equal(3, top3Users.Count);
            Assert.Equal("user4@example.com", top3Users[0].Email); // Highest score
            Assert.Equal("user2@example.com", top3Users[1].Email); // Second highest
            Assert.Equal("user1@example.com", top3Users[2].Email); // Third highest
        }

        [Fact]
        public async Task GetTopUsersAsync_ReturnsAllUsers_WhenTopCountExceedsUserCount()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var users = new List<User>
            {
                new User
                {
                    Email = "userA@example.com",
                    HighScore = 120,
                    GoogleId = "googleUserA",
                    DisplayName = "User A",
                    GivenName = "User",
                    FamilyName = "A",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                },
                new User
                {
                    Email = "userB@example.com",
                    HighScore = 180,
                    GoogleId = "googleUserB",
                    DisplayName = "User B",
                    GivenName = "User",
                    FamilyName = "B",
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.RegisteredUser
                }
            };

            // Create a new scope for resolving AppDbContext
            using (var scope = _fixture.ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Act
            var top5Users = await _userService.GetTopUsersAsync(5);

            // Assert
            Assert.NotNull(top5Users);
            Assert.Equal(2, top5Users.Count);
            Assert.Equal("userB@example.com", top5Users[0].Email); // Highest score
            Assert.Equal("userA@example.com", top5Users[1].Email); // Second highest
        }

        #endregion

        #region UpdateUserAsync Tests

        [Fact]
        public async Task UpdateUserAsync_UpdatesUserPropertiesSuccessfully()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.NameIdentifier, "google987" },
                { ClaimTypes.Email, "updateuser@example.com" },
                { ClaimTypes.GivenName, "Update" },
                { ClaimTypes.Surname, "User" },
                { ClaimTypes.Name, "Update User" },
                { "picture", "http://example.com/update.jpg" }
            };
            var authState = CreateAuthenticationState(true, claims);
            var user = await _userService.GetOrCreateUserFromGoogleAsync(authState);

            // Modify user properties
            user.DisplayName = "Updated User";
            user.CurrentScore = 300;
            user.HighScore = 500;
            user.TotalLessonsCompleted = 20;
            user.TotalStudyTime = TimeSpan.FromHours(15);
            user.CurrentStreak = 5;
            user.BestStreak = 10;

            // Act
            await _userService.UpdateUserAsync(user);

            // Assert
            using (var scope = _fixture.ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                Assert.NotNull(updatedUser);
                Assert.Equal("Updated User", updatedUser.DisplayName);
                Assert.Equal(300, updatedUser.CurrentScore);
                Assert.Equal(500, updatedUser.HighScore);
                Assert.Equal(20, updatedUser.TotalLessonsCompleted);
                Assert.Equal(TimeSpan.FromHours(15), updatedUser.TotalStudyTime);
                Assert.Equal(5, updatedUser.CurrentStreak);
                Assert.Equal(10, updatedUser.BestStreak);
            }
        }

        #endregion
    }
}
