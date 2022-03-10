namespace TaxiCentral.API.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; protected set; }
        public string Surname { get; protected set; }
        public string Pin { get; set; }

        public Driver(string name, string surname, string pin)
        {
            Name = name;
            Surname = surname;
            Pin = pin;
        }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
