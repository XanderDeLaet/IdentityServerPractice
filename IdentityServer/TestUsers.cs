using System.Collections.Generic;
using Duende.IdentityServer.Test;

public static class TestUsers
{
    public static List<TestUser> Users => new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password"
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "bob",
            Password = "password"
        }
    };
}
