﻿namespace CodeShareBackend;

public class RegisterRequestCodeShare
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string UserName { get; init; }
}

