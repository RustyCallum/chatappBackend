﻿using chatappBackend.User;
using chatappBackend.ContactList;
using Microsoft.EntityFrameworkCore;

namespace chatappBackend.Data
{
    public class UserDataContext : DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }
        public DbSet<Users> Users => Set<Users>();
        public DbSet<ContactsList> ContactList => Set<ContactsList>();
        public DbSet<Message.Messages> Messages => Set<Message.Messages>();
    }
}
