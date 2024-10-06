using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Example.Database;

namespace PotluckTest
{
    internal class MockDb : IPotluckDb
    {
        public User? User { get; set; } = null;

        public string? GetUserPassedName { get; private set; } = null;

        public User? GetUser(string? name)
        {
            GetUserPassedName = name;
            return User;
        }

        public int SaveChangesTimesCalled { get; private set; } = 0;

        public int SaveChanges()
        {
            SaveChangesTimesCalled++;
            return 1;
        }
    }
}
