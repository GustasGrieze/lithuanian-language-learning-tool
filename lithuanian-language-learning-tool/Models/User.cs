﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lithuanian_language_learning_tool.Models
{
    public enum UserRole
    {
        Guest,
        RegisteredUser,
        Admin
    }

    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Google Authentication related properties
        public string GoogleId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; }


        // Role management
        public UserRole Role { get; set; } = UserRole.RegisteredUser;
        public bool IsAdmin => Role == UserRole.Admin;
        public bool IsGuest => Role == UserRole.Guest;


        // Application specific properties
        public int CurrentScore { get; set; }
        public int HighScore { get; set; }
        public int TotalLessonsCompleted { get; set; }
        public TimeSpan TotalStudyTime { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }


        // Achievement tracking
        public List<UserAchievement> Achievements { get; set; } = new();


        // Progress tracking
        public Dictionary<string, int> LessonProgress { get; set; } = new();
        public List<PracticeSession> PracticeSessions { get; set; } = new();


        // Learning statistics
        public int CorrectAnswers { get; set; }
        public int TotalAttempts { get; set; }
        public double AccuracyRate => TotalAttempts > 0 ? (double)CorrectAnswers / TotalAttempts * 100 : 0;

        public bool HasPermission(string permission)
        {
            return Role switch
            {
                UserRole.Admin => true, // Admins have all permissions
                UserRole.RegisteredUser => IsRegisteredUserPermission(permission),
                UserRole.Guest => IsGuestPermission(permission),
                _ => false
            };
        }

        private bool IsRegisteredUserPermission(string permission)
        {
            // Define permissions available to registered users
            var registeredUserPermissions = new HashSet<string>
            {
                "view_profile",
                "save_progress",
                "participate_in_lessons",
                "earn_achievements",
                "view_statistics"
            };

            return registeredUserPermissions.Contains(permission);
        }

        private bool IsGuestPermission(string permission)
        {
            // Define permissions available to guests
            var guestPermissions = new HashSet<string>
            {
                "view_public_content",
                "try_demo_lesson"
            };

            return guestPermissions.Contains(permission);
        }
    }

    public struct UserAchievement
    {
        public string AchievementId;
        public string Name;
        public string Description;
        public DateTime UnlockedAt;
    }

    public struct PracticeSession
    {
        public DateTime SessionDate;
        public TimeSpan Duration;
        public int ScoreEarned;
        public string LessonType;       // could be Enum
        public int CorrectAnswers;
        public int TotalQuestions;
    }
}