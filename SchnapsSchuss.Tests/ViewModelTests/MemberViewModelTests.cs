using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;
using SchnapsSchuss.Tests.ViewModels;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Tests.ViewModels
{
    public class MemberViewModelTests
    {
        /*
         * Tests if adding a new member works as expected
         */
        [Fact]
        public async Task AddMember_ShouldAddMember()
        {
            // ARRANGE
            var db = new MemberDatabase(new List<Member>
            {
                new Member { Id = 1, Username = "existingUser", FirstName = "John", LastName = "Doe" }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Mitglieder" },
                { "shownColumns", Member.Columns }
            };

            var viewModel = new CrudViewModel<Member>(query, db);

            // ACT
            var newMember = new Member
            {
                Id = 2,
                Username = "newUser",
                Password = "password123",
                FirstName = "Jane",
                LastName = "Smith",
                Role = RoleType.MEMBER,
                OwnsGunOwnershipCard = true,
                IsRangeSupervisor = false
            };

            viewModel.SelectedItem = newMember;
            viewModel.OnPopupSubmit();

            // ASSERT
            var allMembers = await db.GetAllAsync();

            Assert.Contains(allMembers, m => m.Id == 2 && m.Username == "newUser" && m.FirstName == "Jane" && m.LastName == "Smith");

            // Also assert it is in the ViewModel Items
            Assert.Contains(viewModel.Items, m => m.Id == 2 && m.Username == "newUser");
        }

        /*
         * Tests if loading all members from the database works as expected
         */
        [Fact]
        public async Task LoadMembers_ShouldLoadAllMembers()
        {
            // ARRANGE
            var db = new MemberDatabase(new List<Member>
            {
                new Member { Id = 1, Username = "admin", FirstName = "Admin", LastName = "User", Role = RoleType.ADMINISTRATOR },
                new Member { Id = 2, Username = "john.doe", FirstName = "John", LastName = "Doe", Role = RoleType.MEMBER },
                new Member { Id = 3, Username = "jane.smith", FirstName = "Jane", LastName = "Smith", Role = RoleType.MEMBER }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Mitglieder" },
                { "shownColumns", Member.Columns }
            };

            var viewModel = new CrudViewModel<Member>(query, db);

            // ACT
            viewModel.LoadItems();

            // ASSERT
            var allMembers = await db.GetAllAsync();

            // Check that all members are loaded into ViewModel.Items
            Assert.Equal(allMembers.Count, viewModel.Items.Count);

            // Check that FilteredItems initially matches Items
            Assert.Equal(viewModel.Items.Count, viewModel.FilteredItems.Count);

            // Verify specific members exist
            Assert.Contains(viewModel.Items, m => m.Username == "admin");
            Assert.Contains(viewModel.Items, m => m.Username == "john.doe");
            Assert.Contains(viewModel.Items, m => m.Username == "jane.smith");
        }

        /*
         * Tests if updating Members works as expected.
         */
        [Fact]
        public async Task UpdateMember_ShouldUpdateMember()
        {
            // ARRANGE
            var db = new MemberDatabase(new List<Member>
            {
                new Member { Id = 1, Username = "admin", FirstName = "Admin", LastName = "User", Role = RoleType.ADMINISTRATOR },
                new Member { Id = 2, Username = "john.doe", FirstName = "John", LastName = "Doe", Role = RoleType.MEMBER }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Mitglieder" },
                { "shownColumns", Member.Columns }
            };

            var viewModel = new CrudViewModel<Member>(query, db);

            // ACT
            // Select member to update
            var memberToUpdate = (await db.GetAllAsync()).First(m => m.Id == 2);
            viewModel.SelectedItem = memberToUpdate;

            // Modify some properties
            memberToUpdate.FirstName = "Johnny";
            memberToUpdate.LastName = "Doe-Smith";
            memberToUpdate.Username = "johnny.doe";

            // Submit changes
            viewModel.OnPopupSubmit();

            // ASSERT
            var updatedMembers = await db.GetAllAsync();
            var updatedMember = updatedMembers.First(m => m.Id == 2);

            Assert.Equal("Johnny", updatedMember.FirstName);
            Assert.Equal("Doe-Smith", updatedMember.LastName);
            Assert.Equal("johnny.doe", updatedMember.Username);

            // Also verify it's updated in ViewModel Items
            var vmMember = viewModel.Items.First(m => m.Id == 2);
            Assert.Equal("Johnny", vmMember.FirstName);
            Assert.Equal("Doe-Smith", vmMember.LastName);
            Assert.Equal("johnny.doe", vmMember.Username);
        }
    }
}
