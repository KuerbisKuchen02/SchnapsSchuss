using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;
using SchnapsSchuss.Tests.ViewModels;

namespace SchnapsSchuss.Tests.ViewModels
{
    public class HomePageViewModelTests
    {
        /*
         * Tests if persons can be added to the training.
         * The Person has no open Invoices and is therefore new to the training.
         */
        [Fact]
        public void LoadPersonInTrainingWithNewPerson_ShouldAddPerson()
        {
            // ARRANGE
            // Create all necessary entities
            InvoiceDatabase invoiceDatabase = new InvoiceDatabase(new List<Invoice>());
            InvoiceItemDatabase invoiceItemDatabase = new InvoiceItemDatabase(new List<InvoiceItem>());
            PersonDatabase personDatabase = new PersonDatabase(new List<Person>
            {
                new Person { Id = 1, Role = RoleType.MEMBER }
            });

            Dictionary<string, string> preferences = new Dictionary<string, string>
            {
                { "PersonIds", "" }
            };

            // Create the ViewModel and query attributes
            var viewModel = new HomePageViewModel(personDatabase, invoiceDatabase, invoiceItemDatabase, preferences);
            Dictionary<string, object> queryAttributes = new Dictionary<string, object>();
            queryAttributes.Add("NewPerson", 1);

            // ACT
            // This tells the HomePageViewModel, that one Member has joined the training.
            viewModel.ApplyQueryAttributes(queryAttributes);
            viewModel.LoadPersonList();

            // ASSERT
            Assert.Contains(viewModel.Persons, p => p.Id == 1);
        }

        /*
         * Tests if loading persons which are already present works correctly.
         */
        [Fact]
        public void LoadPersonInTrainingNoNewPewrson_ShouldLoadPresentPersons()
        {
            // ARRANGE
            // Create all necessary entities
            Person person = new Person { Id = 1, Role = RoleType.MEMBER };
            InvoiceDatabase invoiceDatabase = new InvoiceDatabase(new List<Invoice>
            {
                new Invoice
                {
                    Id = 1,
                    Date = DateTime.Now,
                    isPaidFor = false,
                    PersonId = 1,
                    Person = person,
                    InvoiceItems = new List<InvoiceItem>()
                }
            });

            InvoiceItemDatabase invoiceItemDatabase = new InvoiceItemDatabase(new List<InvoiceItem>());
            PersonDatabase personDatabase = new PersonDatabase(new List<Person>{ person });
            Dictionary<string, string> preferences = new Dictionary<string, string>
            {
                { "PersonIds", "[1]" }
            };

            // Create the ViewModel and query attributes
            var viewModel = new HomePageViewModel(personDatabase, invoiceDatabase, invoiceItemDatabase, preferences);
            Dictionary<string, object> queryAttributes = new Dictionary<string, object>();
            
            // ACT
            // This tells the HomePageViewModel, that one Member has joined the training.
            viewModel.ApplyQueryAttributes(queryAttributes);
            viewModel.LoadPersonList();

            // ASSERT
            Assert.Contains(viewModel.Persons, p => p.Id == 1);
        }
    }
}
