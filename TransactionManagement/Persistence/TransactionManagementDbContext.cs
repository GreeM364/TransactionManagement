using Microsoft.EntityFrameworkCore;
using TransactionManagement.Entities;

namespace TransactionManagement.Persistence
{
    /// <summary>
    /// Represents a database context for managing transactions.
    /// </summary>
    public class TransactionManagementDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManagementDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for the context.</param>
        public TransactionManagementDbContext(DbContextOptions<TransactionManagementDbContext> options) : base(options)
        { }

        /// <summary>
        /// Gets or sets the set of transactions.
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }
    }
}
