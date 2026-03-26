using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using youtube_notes.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace youtube_notes.Models;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new youtube_notesContext(
            serviceProvider.GetRequiredService<DbContextOptions<youtube_notesContext>>());

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // DB has been seeded
        if (context.Note.Any())
        {
            return;
        }

        // Create fake users
        var users = new[]
        {
            new { Email = "alice@example.com", UserName = "alice@example.com", Password = "Password1" },
            new { Email = "bob@example.com", UserName = "bob@example.com", Password = "Password1" },
            new { Email = "carol@example.com", UserName = "carol@example.com", Password = "Password1" },
        };

        var createdUsers = new List<IdentityUser>();

        foreach (var u in users)
        {
            var existing = await userManager.FindByEmailAsync(u.Email);
            if (existing != null)
            {
                createdUsers.Add(existing);
                continue;
            }

            var user = new IdentityUser { UserName = u.UserName, Email = u.Email };
            var result = await userManager.CreateAsync(user, u.Password);
            if (result.Succeeded)
            {
                createdUsers.Add(user);
            }
        }

        var youtubeId = "cyiWMIE3HGg";

        context.Note.AddRange(
            // Alice's comments
            new Note
            {
                Comment = "Great explanation of this concept, very clear!",
                YoutubeId = youtubeId,
                TimeAt = 45,
                UserId = createdUsers[0].Id
            },
            new Note
            {
                Comment = "I had to rewatch this part a few times but it finally clicked.",
                YoutubeId = youtubeId,
                TimeAt = 180,
                UserId = createdUsers[0].Id
            },
            // Bob's comments
            new Note
            {
                Comment = "This is a really useful tutorial, bookmarking for later.",
                YoutubeId = youtubeId,
                TimeAt = 10,
                UserId = createdUsers[1].Id
            },
            new Note
            {
                Comment = "The example at this timestamp helped me understand the pattern.",
                YoutubeId = youtubeId,
                TimeAt = 300,
                UserId = createdUsers[1].Id
            },
            // Carol's comments
            new Note
            {
                Comment = "Would love to see a follow-up video on this topic.",
                YoutubeId = youtubeId,
                TimeAt = 120,
                UserId = createdUsers[2].Id
            },
            new Note
            {
                Comment = "Nice breakdown, the visuals really help.",
                YoutubeId = youtubeId,
                TimeAt = 240,
                UserId = createdUsers[2].Id
            }
        );

        await context.SaveChangesAsync();
    }
}
