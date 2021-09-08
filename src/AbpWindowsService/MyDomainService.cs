using System;
using System.IO;
using AbpWindowsService.Books;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AbpWindowsService
{
    public class MyDomainService : ITransientDependency
    {
        private readonly ILogger<MyDomainService> _logger;
        private readonly IBookRepository _bookRepository;

        public MyDomainService(ILogger<MyDomainService> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public void StartService()
        {
            _logger.LogInformation("Service started");

            GetBooks();
        }

        public void StopService()
        {
            _logger.LogInformation("Service stopped.");
        }

        private void GetBooks()
        {
            _logger.LogInformation(Environment.NewLine + "GETTING BOOKS ..." + Environment.NewLine);

            var books = _bookRepository.GetListAsync().Result;
            foreach (var book in books)
            {
                _logger.LogInformation("*** Book name: " + book.Name + Environment.NewLine);
            }
        }

    }
}
