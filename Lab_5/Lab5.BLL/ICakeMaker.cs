namespace Lab5.BLL;

/// <summary>Операція: готувати тістечка з різних інгредієнтів.</summary>
public interface ICakeMaker
{
    /// <summary>Готує тістечко з переданих інгредієнтів.</summary>
    string MakeCake(params string[] ingredients);
}
