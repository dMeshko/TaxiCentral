using System;
using TaxiCentral.API.Models;

namespace TaxiCentral.UnitTests.Fixtures
{
    public static class DriversFixture
    {
        public static Driver GetFakeDriver()
        {
            return new Driver("Darko", "Meshkovski", "1234")
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };
        }
    }
}
