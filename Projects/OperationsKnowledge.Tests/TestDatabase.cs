using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OperationsKnowledge.Data;

namespace OperationsKnowledge.Tests;

internal class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    public OperationalSystemContext Context { get; }

    public TestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<OperationalSystemContext>().UseSqlite(_connection).Options;
        Context = new OperationalSystemContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
