using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moe.Core.Models.Entities;
using Moe.Core.Data.Interceptors;
using Moe.Core.Models.Entities.Moe.Core.Models.Entities;

namespace Moe.Core.Data;

public class MasterDbContext : DbContext    
{
    public readonly IMapper _mapper;
    public MasterDbContext(DbContextOptions<MasterDbContext> options, IMapper mapper) : base(options)
    {
        _mapper = mapper;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PendingUser> PendingUsers { get; set; }

    public DbSet<LocalizedContent> LocalizedContents { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    //{{INSERTION_POINT}}  
    public DbSet<WarehouseItemTransaction> WarehouseItemTransactions { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }

    public DbSet<ChangePasswordRequest> ChangePasswordRequest { get; set; }

    public DbSet<ChangeEmailRequest> ChangeEmailRequest { get; set; }


    public DbSet<ChangePhoneRequest> ChangePhoneRequest { get; set; }

    public DbSet<WarehouseItem> WarehouseItems { get; set; }    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(e => e.Actor)
                .WithMany(e => e.NotificationsSent)
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<WarehouseItem>(entity =>
        {

            entity.HasOne(e => e.Warehouse)
                  .WithMany(w => w.WarehouseItems)
                  .HasForeignKey(e => e.WarehouseId);

            entity.HasOne(e => e.Item)
                  .WithMany(i => i.WarehouseItems)
                  .HasForeignKey(e => e.ItemId);
        });

        modelBuilder.Entity<WarehouseItemTransaction>(entity =>
        {
       
            entity.HasOne(e => e.FromWarehouse)
                  .WithMany()
                  .HasForeignKey(e => e.FromWarehouseId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToWarehouse)
                  .WithMany()
                  .HasForeignKey(e => e.ToWarehouseId)
                  .OnDelete(DeleteBehavior.Restrict);
          
            entity.HasOne(e => e.Item)
                  .WithMany()
                  .HasForeignKey(e => e.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}

//Restrictions:
//  Username    => MaxLength(16)
//  Name    => MaxLength(128)

//  Email   => MaxLength(64)
//  Link    => MaxLength(256)
//  PhoneNumber    => MaxLength(16)
//  PhoneCountryCode    => MaxLength(3)

//  Title    => MaxLength(128)
//  About    => MaxLength(128)
//  Content    => MaxLength(512) (Short content)
//  Content    => MaxLength(1024) (Long content)
//  Description    => MaxLength(1024)
//  Notes    => MaxLength(1024)

//  Attachment    => MaxLength(128)
//  Attachments    => MaxLength(32)
