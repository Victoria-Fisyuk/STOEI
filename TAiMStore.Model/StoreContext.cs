using System;
using System.Data.Entity;
using System.Linq;
using TAiMStore.Domain;

namespace TAiMStore.Model
{
    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<PaymentType> PaymentType { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Role> Roles { get; set; }

        public override int SaveChanges()
        {
            foreach (var source in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                source.Property("CreateDate").CurrentValue = DateTime.Now;
            }
            foreach (var source in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                source.Property("ModifiedDate").CurrentValue = DateTime.Now;
            }
            return base.SaveChanges();
        }

        public virtual void Commit()
        {
            SaveChanges();
        }
    }
}
