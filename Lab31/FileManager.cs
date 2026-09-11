using System;
using System.IO;

class FileManager
{
    private string fileName;

    public FileManager(string fileName)
    {
        this.fileName = fileName;
    }

    public void WriteStudent(Student student)
    {
        StreamWriter writer = null;

        try
        {
            writer = new StreamWriter(fileName, true);

            writer.WriteLine("Student " +
                student.FirstName + student.LastName);

            writer.WriteLine("{");
            writer.WriteLine("\"firstname\": \"" +
                student.FirstName + "\",");

            writer.WriteLine("\"lastname\": \"" +
                student.LastName + "\",");

            writer.WriteLine("\"course\": \"" +
                student.Course + "\",");

            writer.WriteLine("\"studentId\": \"" +
                student.StudentId + "\",");

            writer.WriteLine("\"birthDate\": \"" +
                student.BirthDate + "\"");

            writer.WriteLine("};");
        }
        catch (IOException exc)
        {
            Console.WriteLine("Помилка вводу-виводу:");
            Console.WriteLine(exc.Message);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }

    public Student[] ReadStudents()
    {
        Student[] students = new Student[100];
        int count = 0;

        StreamReader reader = null;

        try
        {
            reader = new StreamReader(fileName);

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("Student "))
                {
                    reader.ReadLine();

                    string firstName =
                        GetValue(reader.ReadLine());

                    string lastName =
                        GetValue(reader.ReadLine());

                    int course =
                        int.Parse(GetValue(reader.ReadLine()));

                    string studentId =
                        GetValue(reader.ReadLine());

                    string birthDate =
                        GetValue(reader.ReadLine());

                    reader.ReadLine();

                    students[count] = new Student(
                        firstName,
                        lastName,
                        course,
                        studentId,
                        birthDate);

                    count++;
                }
            }
        }
        catch (IOException exc)
        {
            Console.WriteLine("Помилка вводу-виводу:");
            Console.WriteLine(exc.Message);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }

        Student[] result = new Student[count];

        for (int i = 0; i < count; i++)
        {
            result[i] = students[i];
        }

        return result;
    }

    private string GetValue(string line)
    {
        int firstQuote = line.IndexOf("\"", line.IndexOf(":") + 1);
        int lastQuote = line.LastIndexOf("\"");

        return line.Substring(
            firstQuote + 1,
            lastQuote - firstQuote - 1);
    }
}