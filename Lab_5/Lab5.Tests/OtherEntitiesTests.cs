using Lab5.BLL;

namespace Lab5.Tests;

[TestClass]
public class OtherEntitiesTests
{
    [TestMethod]
    public void Baker_MakeCake_ReturnsDistinctIngredients()
    {
        // Arrange
        var baker = new Baker("Петренко", "Оксана");

        // Act
        string cake = baker.MakeCake("Борошно", "цукор", "борошно");

        // Assert
        Assert.AreEqual("Пекар Оксана приготував(ла) тістечко з: борошно, цукор", cake);
    }

    [TestMethod]
    public void Baker_MakeCake_OneIngredient_Throws()
    {
        // Arrange
        var baker = new Baker("Петренко", "Оксана");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => baker.MakeCake("цукор", "Цукор"));
    }

    [TestMethod]
    public void Entrepreneur_MakeCake_EmptyIngredient_Throws()
    {
        // Arrange
        var entrepreneur = new Entrepreneur("Іваненко", "Дмитро", "Лавка");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => entrepreneur.MakeCake("цукор", ""));
    }

    [TestMethod]
    public void Entrepreneur_MakeCake_ContainsBusiness()
    {
        // Arrange
        ICakeMaker maker = new Entrepreneur("Іваненко", "Дмитро", "Лавка");

        // Act
        string cake = maker.MakeCake("шоколад", "яйця");

        // Assert
        StringAssert.Contains(cake, "(Лавка)");
    }

    [TestMethod]
    public void Entrepreneur_EmptyBusiness_Throws()
    {
        // Arrange, Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new Entrepreneur("Іваненко", "Дмитро", " "));
    }

    [TestMethod]
    public void Baker_JumpWithParachute_UsesSharedSkill()
    {
        // Arrange
        ISkydiver skydiver = new Baker("Петренко", "Оксана");

        // Act
        string result = skydiver.JumpWithParachute();

        // Assert
        Assert.AreEqual("Оксана Петренко стрибає з парашутом!", result);
    }

    [TestMethod]
    public void Baker_ToString_ReturnsFullName()
    {
        // Arrange
        var baker = new Baker("Петренко", "Оксана");

        // Act
        string text = baker.ToString();

        // Assert
        Assert.AreEqual("Петренко Оксана", text);
    }

    [TestMethod]
    public void MakeCake_NullIngredients_Throws()
    {
        // Arrange
        var baker = new Baker("Петренко", "Оксана");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => baker.MakeCake(null!));
    }
}
