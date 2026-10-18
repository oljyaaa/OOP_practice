using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Repositories;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class RepositoryTests
    {
        private static Author CreateAuthor(string lastName) =>
            new() { FirstName = "Іван", LastName = lastName, Email = $"{Guid.NewGuid():N}@mail.com", RoyaltyPercent = 10 };

        [TestMethod]
        public void Add_ThenGetById_ReturnsSameItem()
        {
            var repo = new Repository<Author>("Автор");
            var author = CreateAuthor("Франко");

            repo.Add(author);

            Assert.AreSame(author, repo.GetById(author.Id));
            Assert.AreEqual(1, repo.Count);
        }

        [TestMethod]
        public void Add_DuplicateId_Throws()
        {
            var repo = new Repository<Author>("Автор");
            var author = CreateAuthor("Франко");
            repo.Add(author);

            Assert.ThrowsExactly<InvalidOperationException>(() => repo.Add(author));
        }

        [TestMethod]
        public void GetById_Missing_ThrowsEntityNotFound()
        {
            var repo = new Repository<Author>("Автор");

            var ex = Assert.ThrowsExactly<EntityNotFoundException>(() => repo.GetById(Guid.NewGuid()));
            Assert.AreEqual("Автор", ex.EntityName);
        }

        [TestMethod]
        public void Remove_DeletesFromListAndIndex()
        {
            var repo = new Repository<Author>("Автор");
            var author = CreateAuthor("Франко");
            repo.Add(author);

            Assert.IsTrue(repo.Remove(author.Id));
            Assert.IsFalse(repo.Remove(author.Id));
            Assert.AreEqual(0, repo.Count);
            Assert.IsFalse(repo.TryGetById(author.Id, out _));
        }

        [TestMethod]
        public void Find_FiltersByPredicate()
        {
            var repo = new Repository<Author>("Автор");
            repo.Add(CreateAuthor("Франко"));
            repo.Add(CreateAuthor("Шевченко"));

            var found = repo.Find(a => a.LastName.StartsWith("Ш"));

            Assert.HasCount(1, found);
            Assert.AreEqual("Шевченко", found[0].LastName);
        }

        [TestMethod]
        public void ReplaceAll_WithDuplicates_KeepsOldData()
        {
            var repo = new Repository<Author>("Автор");
            var existing = CreateAuthor("Франко");
            repo.Add(existing);
            var duplicate = CreateAuthor("Леся");

            Assert.ThrowsExactly<InvalidOperationException>(() => repo.ReplaceAll(new[] { duplicate, duplicate }));
            Assert.AreSame(existing, repo.GetById(existing.Id));
            Assert.AreEqual(1, repo.Count);
        }
    }
}
