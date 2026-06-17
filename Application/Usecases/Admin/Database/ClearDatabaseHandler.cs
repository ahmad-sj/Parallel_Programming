using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Database;

public class ClearDatabaseHandler
{
    IRepository _repository;

    public ClearDatabaseHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ClearDatabaseCommand command)
    {
        // Dynamic script to bypass constraints, empty everything, and reset identity tracking
        string sqlServerTeardownScript = @"
            -- Step 1: Broadly disable foreign key check constraints across the entire DB
            EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

            -- Step 2: Wipe data. 
            -- 'DELETE' is used here via loop to ensure tables targeted by foreign keys clear safely, 
            -- while avoiding manually ordered script files.
            -- EXEC sp_MSforeachtable 'DELETE FROM ?';

               EXEC sp_MSforeachtable 'DELETE FROM Orders';
               EXEC sp_MSforeachtable 'DELETE FROM OrderItems';
               EXEC sp_MSforeachtable 'DELETE FROM Carts';
               EXEC sp_MSforeachtable 'DELETE FROM CartItems';

            -- Step 3: Reset the Identity auto-increment keys back to 1 for all relevant tables
            EXEC sp_MSforeachtable '
                IF OBJECTPROPERTY(OBJECT_ID(''?''), ''TableHasIdentity'') = 1 
                BEGIN
                    DBCC CHECKIDENT (''?'', RESEED, 0);
                END
            ';

            -- Step 4: Re-enable foreign key constraints for integrity consistency
            EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
        ";

        await _repository.ExecuteSqlCommandAsync(sqlServerTeardownScript);

        // Clears stored data pages and execution plan caches for pristine next-run benchmarking
        //await _repository.ExecuteSqlCommandAsync("DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;");

        // Force clears EF Core tracker cache so it doesn't try to reuse old memory references
        //_repository.ResetContextState();
    }
}
