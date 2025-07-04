using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;

namespace ContactSearchWorkflow.Tests
{
    public class ContactSearchTests
    {
        private readonly XrmFakedContext _context;

        public ContactSearchTests()
        {
            _context = new XrmFakedContext();
        }

        [Fact]
        public void Returns_Status_1_When_One_Contact_Found()
        {
            var contact = new Entity("Contact")
            {
                Id = Guid.NewGuid(),
                ["FirstName"] = "Andrew",
                ["LastName"] = "Marchenko"
            };

            _context.Initialize(new List<Entity> { contact });

            var inputs = new Dictionary<string, object>
            {
                { "SearchValue1", "Andrew" },
                { "FirstName", "FirstName" },
                { "SearchValue2", "Marchenko" },
                { "LastName", "LastName" }
            };

            var result = _context.ExecuteCodeActivity<ContactSearch>(inputs);

            Assert.Equal(1, result["Status"]);
            var reference = result["FoundContact"] as EntityReference;
            Assert.NotNull(reference);
            Assert.Equal(contact.Id, reference.Id);
        }

        [Fact]
        public void Returns_Status_2_When_No_Contact_Found()
        {
            _context.Initialize(new List<Entity>());

            var inputs = new Dictionary<string, object>
            {
                { "SearchValue1", "Andrew" },
                { "FirstName", "FirstName" },
                { "SearchValue2", "Marchenko" },
                { "LastName", "LastName" }
            };

            var result = _context.ExecuteCodeActivity<ContactSearch>(inputs);

            Assert.Equal(2, result["Status"]);
            Assert.Null(result["FoundContact"]);
        }

        [Fact]
        public void Returns_Status_3_When_Multiple_Contacts_Found()
        {
            var contact1 = new Entity("Contact")
            {
                Id = Guid.NewGuid(),
                ["FirstName"] = "Andrew",
                ["LastName"] = "Marchenko"
            };

            var contact2 = new Entity("Contact")
            {
                Id = Guid.NewGuid(),
                ["FirstName"] = "Andrew",
                ["LastName"] = "Marchenko"
            };

            _context.Initialize(new List<Entity> { contact1, contact2 });

            var inputs = new Dictionary<string, object>
            {
                { "SearchValue1", "Andrew" },
                { "FirstName", "FirstName" },
                { "SearchValue2", "Marchenko" },
                { "LastName", "LastName" }
            };

            var result = _context.ExecuteCodeActivity<ContactSearch>(inputs);

            Assert.Equal(3, result["Status"]);
            Assert.Null(result["FoundContact"]);
        }
    }
}
