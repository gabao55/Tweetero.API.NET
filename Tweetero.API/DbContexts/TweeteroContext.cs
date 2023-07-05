﻿using Microsoft.EntityFrameworkCore;
using Tweetero.API.Entities;

namespace Tweetero.API.DbContexts
{
    public class TweeteroContext : DbContext
    {
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<User> Users { get; set; }

        public TweeteroContext(DbContextOptions<TweeteroContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasData(
                    new User()
                    {
                        Id = 1,
                        Username = "test",
                        Password = "123456",
                        Avatar = "https://www.racoesreis.com.br/wordpress/wp-content/uploads/cachorro-origem3.jpg"
                    }
                );
            modelBuilder.Entity<Tweet>()
                .HasData(
                    new Tweet("I love crunchy lettuce!")
                    {
                        Id = 1,
                        UserId = 1
                    },
                    new Tweet("I hate when I get alone... :(")
                    {
                        Id = 2,
                        UserId = 1
                    }
                );
        }
    }
}