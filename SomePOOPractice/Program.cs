using System;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;

namespace SomePOOPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hola! ¿Cómo te llamas?");
            string currentUserName = Console.ReadLine();
            User user = new User(currentUserName);
            Console.WriteLine($"Hola, {user.Nombre}!");

            List<User> list = ReadJSON(currentUserName);
            //  if JSON is empty, it will create a list anyway:
            if (list == null) { list = new List<User>(); }

            if (!UserExists(user, list, out user))
            {
                char rpta;
                do
                {
                    user.FechaNacimiento = ReadDate();
                    Console.WriteLine("Si tus datos correctos, ingresa S; de lo contrario, cualquier otra letra");
                    try
                    {
                        rpta = Char.Parse(Console.ReadLine().ToUpper());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Lo siento, no hemos podido leer tu respuesta.");
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("Inténtalo de nuevo.");
                        rpta = 'N';
                    }
                } while (rpta != 'S');
                SaveData(user, list);
            }
            else
            {
                Console.WriteLine("Hemos encontrado tus datos!");
                Console.WriteLine(user.ToString());
            }

            TimeSpan userAge = DateTime.Now.Subtract(user.FechaNacimiento);

            int option;
            do
            {
                option = ShowMenu();
                ShowResults(userAge, option);
            } while (option != 5);

            Console.WriteLine($"Vuelve pronto!");
            Console.WriteLine("Fecha actual: " + DateTime.Now);
            Console.WriteLine("ENTER para salir...");
            Console.ReadLine();
        }

        static List<User> ReadJSON(string name)
        {
            string JSONstring = File.ReadAllText("json1.json");
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<User> list = ser.Deserialize<List<User>>(JSONstring);
            return list;
        }
        static bool UserExists(User anotherUser, List<User> list, out User user)
        {
            foreach (var userSaved in list)
            {
                if (userSaved.Equals(anotherUser))
                {
                    user = userSaved;
                    return true;
                }
            }
            user = anotherUser;
            return false;
        }
        static DateTime ReadDate()
        {
            DateTime date = new DateTime();
            try
            {
                Console.WriteLine("Necesitamos algunos datos tuyos para continuar :)");
                Console.WriteLine("¿Cuándo naciste?");
                Console.WriteLine("Día: ");
                int day = Int16.Parse(Console.ReadLine());

                Console.WriteLine("Mes: (1-12)");
                int month = Int16.Parse(Console.ReadLine());

                Console.WriteLine("Año: (XXXX)");
                int year = Int16.Parse(Console.ReadLine());

                date = new DateTime(year, month, day);
                return date;
            }
            catch
            {
                Console.WriteLine("Perdón, al parecer ocurrió algún error con la fecha :(");
                Console.WriteLine("ENTER para salir...");
                Console.ReadLine();
                Environment.Exit(0);
            }
            return date;
        }
        static void SaveData(User user, List<User> list)
        {
            char rpta;
            Console.WriteLine("¿Deseas guardar tus datos? Si es así, ingresa S.");
            Console.WriteLine(user.ToString());
            try
            {
                rpta = Char.Parse(Console.ReadLine().ToUpper());
            }
            catch
            {
                Console.WriteLine("No has ingresado una opción válida.");
                Console.WriteLine("Solo se permite el ingreso de una letra a la vez.");
                rpta = 'N';
            }

            if (rpta == 'S')
            {
                //  Capital Case
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                user.Nombre = ti.ToTitleCase(user.Nombre);
                //  Save new JSON
                list.Add(user);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                string cadenaJSON = ser.Serialize(list);
                File.WriteAllText("json1.json", cadenaJSON);
                Console.WriteLine("Guardado exitosamente!");
            }
            else
            {
                Console.WriteLine("Ok! No guardaremos tus datos esta vez.");
            }
        }
        static void ShowResults(TimeSpan userAge, int opc)
        {
            switch (opc)
            {
                case 1:
                    Console.WriteLine($"Has vivido {userAge.Days / 30} meses");
                    break;
                case 2:
                    Console.WriteLine($"Has vivido {userAge.Days} dias");
                    break;
                case 3:
                    Console.WriteLine($"Has vivido aproximadamente {(int)userAge.TotalHours} horas");
                    break;
                case 4:
                    Console.WriteLine($"Has vivido aproximadamente {userAge.TotalSeconds} segundos");
                    break;
                case 5:
                    Console.WriteLine("Gracias por usar la aplicación!");
                    break;
                default:
                    Console.WriteLine("No ingresaste una opción válida :/.");
                    break;
            }
        }
        static int ShowMenu()
        {
            Console.WriteLine("¿Qué deseas saber? ");
            Console.WriteLine("1. ¿Cuántos meses he vivido?");
            Console.WriteLine("2. ¿Cuántos días he vivido?");
            Console.WriteLine("3. ¿Cuántas horas he vivido?");
            Console.WriteLine("4. ¿Cuántos segundos he vivido?");
            Console.WriteLine("5. SALIR");
            try
            {
                return Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Uy! No puedo leer esto.");
                return 5;
            }
        }
    }

    class User : IEquatable<User>
    {
        //  Constructors
        public User() { }
        public User(string Nombre) { this.Nombre = Nombre; }

        //  Properties
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }

        //  Methods
        public override string ToString()
        {
            return $"Nombre: {Nombre} \nFecha de nacimiento: {FechaNacimiento}";
        }
        public bool Equals(User otroUsuario)
        {
            return (this.Nombre.ToUpper().Equals(otroUsuario.Nombre.ToUpper()));
        }
    }
}