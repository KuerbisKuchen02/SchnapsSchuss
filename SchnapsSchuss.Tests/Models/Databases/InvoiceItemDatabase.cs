using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.Models.Databases
{
    public class InvoiceItemDatabase : Database<InvoiceItem>
    {
        private readonly List<InvoiceItem> _invoiceItems;

        public InvoiceItemDatabase(List<InvoiceItem> invoiceItems = null)
        {
            _invoiceItems = invoiceItems ?? new List<InvoiceItem>();
        }

        public Task<int> DeleteAsync(InvoiceItem entity)
        {
            _invoiceItems.Remove(entity);
            return Task.FromResult(1); // return 1 = deleted
        }

        public Task<List<InvoiceItem>> GetAllAsync()
        {
            return Task.FromResult(_invoiceItems.ToList());
        }

        public Task<InvoiceItem> GetOneAsync(int id)
        {
            var invoiceItem = _invoiceItems.FirstOrDefault(ii => ii.Id == id);
            return Task.FromResult(invoiceItem);
        }

        public Task<int> SaveAsync(InvoiceItem entity)
        {
            var existing = _invoiceItems.FirstOrDefault(ii => ii.Id == entity.Id);
            if (existing != null)
            {
                _invoiceItems.Remove(existing); // replace
            }
            _invoiceItems.Add(entity);
            return Task.FromResult(1); // return 1 = saved
        }

        public Task<List<InvoiceItem>> GetInvoiceItemsOfInvoiceAsync(Invoice invoice)
        {
            if (invoice == null)
                return Task.FromResult(new List<InvoiceItem>());

            var items = _invoiceItems
                .Where(ii => ii.InvoiceId == invoice.Id)
                .ToList();

            return Task.FromResult(items);
        }
    }
}
