using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.Models.Databases
{
    public class InvoiceDatabase : Database<Invoice>
    {
        private readonly List<Invoice> _invoices;

        public InvoiceDatabase(List<Invoice> invoices = null)
        {
            _invoices = invoices ?? new List<Invoice>();
        }

        public Task<int> DeleteAsync(Invoice entity)
        {
            _invoices.Remove(entity);
            return Task.FromResult(1); // 1 = deleted
        }

        public Task<List<Invoice>> GetAllAsync()
        {
            return Task.FromResult(_invoices.ToList());
        }

        public Task<Invoice> GetOneAsync(int id)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(invoice);
        }

        public Task<int> SaveAsync(Invoice entity)
        {
            var existing = _invoices.FirstOrDefault(i => i.Id == entity.Id);
            if (existing != null)
            {
                _invoices.Remove(existing); // replace existing
            }
            _invoices.Add(entity);
            return Task.FromResult(1); // 1 = saved
        }

        public Task<Invoice> GetOpenInvoiceForPerson(int personId)
        {
            var openInvoice = _invoices
                .FirstOrDefault(i => i.PersonId == personId && !i.isPaidFor);

            return Task.FromResult(openInvoice);
        }
    }
}