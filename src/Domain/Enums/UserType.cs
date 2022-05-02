using System;

namespace MGCap.Domain.Enums
{
    [Flags]
    public enum UserType
    {
        Employee = 0,
        Customer = 1, // Referes to "Management Companies"
    }
}
