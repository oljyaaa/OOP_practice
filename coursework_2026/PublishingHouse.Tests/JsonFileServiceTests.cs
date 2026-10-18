using PublishingHouse.Core.Exceptions;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;
using PublishingHouse.Core.Services;

namespace PublishingHouse.Tests
{
    [TestClass]
    public sealed class JsonFileServiceTests
    {
        private string _path = null!;

        [TestInitialize]
        public void SetUp() => _path = Path.Combine(Path.GetTempPath(), $"publishing_{Guid.NewGuid():N}.json");

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(_path))
                File.Delete(_path);
        }

        [TestMethod]
        public void Load_MissingFile_ReturnsEmptyData()
        {
            PublishingData data = new JsonFileService<PublishingData>(_path).Load();

            Assert.IsEmpty(data.Authors);
        }

        [TestMethod]
        public void SaveThenLoad_RoundTripsAllEntities()
        {
            var source = new PublishingService();
            var author = new Author { FirstName = "Тарас", LastName = "Шевченко", Email = "taras@mail.com", RoyaltyPercent = 15 };
            var editor = new Editor { FirstName = "Марко", LastName = "Вовчок", Email = "marko@mail.com", RatePerPage = 8, Specialization = Genre.Poetry };
            source.AddAuthor(author);
            source.AddEditor(editor);
            Manuscript m = source.SubmitManuscript(author.Id, "Кобзар", Genre.Poetry, 300);
            source.AssignEditor(m.Id, editor.Id);

            var storage = new JsonFileService<PublishingData>(_path);
            storage.Save(source.ExportData());

            var restored = new PublishingService();
            restored.ImportData(storage.Load());

            Manuscript loaded = restored.Manuscripts.GetById(m.Id);
            Assert.AreEqual("Кобзар", loaded.Title);
            Assert.AreEqual(editor.Id, loaded.EditorId);
            Assert.AreEqual("Шевченко", restored.Authors.GetById(author.Id).LastName);
            StringAssert.Contains(File.ReadAllText(_path), "\"Genre\": \"Poetry\"");
            StringAssert.Contains(File.ReadAllText(_path), "Кобзар");
        }

        [TestMethod]
        public void Load_CorruptedJson_ThrowsDataStorageException()
        {
            File.WriteAllText(_path, "{ not json");

            Assert.ThrowsExactly<DataStorageException>(() => new JsonFileService<PublishingData>(_path).Load());
        }

        [TestMethod]
        public void Load_InvalidFieldValue_ThrowsDataStorageException()
        {
            File.WriteAllText(_path, "{ \"Authors\": [ { \"FirstName\": \"\", \"LastName\": \"X\", \"Email\": \"a@b.com\" } ] }");

            Assert.ThrowsExactly<DataStorageException>(() => new JsonFileService<PublishingData>(_path).Load());
        }
    
        [TestMethod]
        public void Save_DoesNotWriteComputedProperties()
        {
            var data = new PublishingData
            {
                Authors = { new Author { FirstName = "Іван", LastName = "Франко", Email = "ivan@mail.com" } }
            };

            new JsonFileService<PublishingData>(_path).Save(data);
            string json = File.ReadAllText(_path);

            Assert.DoesNotContain("FullName", json);
            Assert.DoesNotContain("Role", json);
            Assert.IsFalse(File.Exists(_path + ".tmp"));
        }

        [TestMethod]
        public void Save_CreatesMissingDirectory()
        {
            string directory = Path.Combine(Path.GetTempPath(), $"publishing_dir_{Guid.NewGuid():N}");
            string path = Path.Combine(directory, "data.json");
            try
            {
                new JsonFileService<PublishingData>(path).Save(new PublishingData());

                Assert.IsTrue(File.Exists(path));
            }
            finally
            {
                Directory.Delete(directory, recursive: true);
            }
        }

        [TestMethod]
        public void Load_EmptyFile_ReturnsEmptyData()
        {
            File.WriteAllText(_path, "   ");

            Assert.IsEmpty(new JsonFileService<PublishingData>(_path).Load().Manuscripts);
        }

        [TestMethod]
        public void Constructor_EmptyPath_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new JsonFileService<PublishingData>(""));
        }
    }
}
