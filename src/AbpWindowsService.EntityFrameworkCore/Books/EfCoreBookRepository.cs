using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AbpWindowsService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AbpWindowsService.Books
{
    public class EfCoreBookRepository : EfCoreRepository<AbpWindowsServiceDbContext, Book, Guid>, IBookRepository
    {
        public EfCoreBookRepository(IDbContextProvider<AbpWindowsServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<Book>> GetListAsync()
        {
            return await this.ToListAsync();
        }
    }
}