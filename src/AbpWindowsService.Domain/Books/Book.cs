using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace AbpWindowsService.Books
{
    public class Book : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string Name { get; set; }

        public Book()
        {

        }

        public Book(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}